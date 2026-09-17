using System.Text;
using System.Text.Json;
using FlashCart.Application.Common.Interfaces;
using FlashCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlashCart.NotificationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(
                "amqp://guest:guest@localhost:5672/")
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        // Main exchange
        await channel.ExchangeDeclareAsync(
            exchange: "flashcart.events",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Main queue
        await channel.QueueDeclareAsync(
            queue: "flashcart.order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "flashcart.order-created",
            exchange: "flashcart.events",
            routingKey: "order.created");

        // DLQ exchange
        await channel.ExchangeDeclareAsync(
            exchange: "flashcart.dlx",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // Notification DLQ
        await channel.QueueDeclareAsync(
            queue: "flashcart.notification-order-created.dlq",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "flashcart.notification-order-created.dlq",
            exchange: "flashcart.dlx",
            routingKey: "notification.failed");

        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body.ToArray();

            int retryCount = 0;

            try
            {
                // Read retry count
                if (args.BasicProperties.Headers != null &&
                    args.BasicProperties.Headers.TryGetValue(
                        "x-retry-count",
                        out var retryHeader))
                {
                    retryCount =
                        Convert.ToInt32(retryHeader);
                }

                var json =
                    Encoding.UTF8.GetString(body);

                var orderCreatedEvent =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(
                        json);

                if (orderCreatedEvent == null)
                {
                    throw new InvalidOperationException(
                        "Invalid OrderCreatedEvent.");
                }

                _logger.LogInformation(
                    "Processing Order #{OrderId}. EventId: {EventId}. Attempt: {Attempt}",
                    orderCreatedEvent.OrderId,
                    orderCreatedEvent.EventId,
                    retryCount + 1);

                // Create a database scope for this message
                using var scope =
                    _scopeFactory.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<IApplicationDbContext>();

                const string consumerName =
                    "NotificationWorker";

                // Check duplicate
                var alreadyProcessed =
                    await dbContext.ProcessedMessages
                        .AnyAsync(
                            x =>
                                x.EventId ==
                                orderCreatedEvent.EventId
                                &&
                                x.ConsumerName ==
                                consumerName);

                if (alreadyProcessed)
                {
                    _logger.LogWarning(
                        "Duplicate event detected. EventId: {EventId}. Skipping notification.",
                        orderCreatedEvent.EventId);

                    await channel.BasicAckAsync(
                        args.DeliveryTag,
                        false);

                    return;
                }

                // -----------------------------------------
                // BUSINESS PROCESSING
                // -----------------------------------------

                _logger.LogInformation(
                    "Sending notification for Order #{OrderId}",
                    orderCreatedEvent.OrderId);

                // Simulate notification processing
                // Example:
                // await _emailService.SendOrderConfirmationAsync(...);

                // -----------------------------------------
                // RECORD EVENT AS PROCESSED
                // -----------------------------------------

                dbContext.ProcessedMessages.Add(
                    new ProcessedMessage
                    {
                        EventId =
                            orderCreatedEvent.EventId,

                        ConsumerName =
                            consumerName,

                        ProcessedAt =
                            DateTime.UtcNow
                    });

                await dbContext.SaveChangesAsync();

                // ACK only after successful processing
                await channel.BasicAckAsync(
                    args.DeliveryTag,
                    false);

                _logger.LogInformation(
                    "Notification processing completed for Order #{OrderId}.",
                    orderCreatedEvent.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Notification processing failed. Retry count: {RetryCount}",
                    retryCount);

                const int maxRetries = 3;

                if (retryCount < maxRetries)
                {
                    var nextRetryCount =
                        retryCount + 1;

                    _logger.LogWarning(
                        "Retrying notification message. Retry count: {RetryCount}",
                        nextRetryCount);

                    await PublishRetryMessageAsync(
                        channel,
                        body,
                        nextRetryCount);

                    // ACK original
                    await channel.BasicAckAsync(
                        args.DeliveryTag,
                        false);
                }
                else
                {
                    _logger.LogError(
                        "Maximum retries reached. Sending notification message to DLQ.");

                    await channel.BasicPublishAsync(
                        exchange: "flashcart.dlx",
                        routingKey: "notification.failed",
                        mandatory: false,
                        basicProperties: args.BasicProperties,
                        body: body);

                    // Remove original from main queue
                    await channel.BasicAckAsync(
                        args.DeliveryTag,
                        false);
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: "flashcart.order-created",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Notification consumer started.");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task PublishRetryMessageAsync(
        IChannel channel,
        ReadOnlyMemory<byte> body,
        int retryCount)
    {
        var properties = new BasicProperties
        {
            Persistent = true,

            Headers =
                new Dictionary<string, object?>
                {
                    ["x-retry-count"] =
                        retryCount
                }
        };

        await channel.BasicPublishAsync(
            exchange: "flashcart.events",
            routingKey: "order.created",
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}