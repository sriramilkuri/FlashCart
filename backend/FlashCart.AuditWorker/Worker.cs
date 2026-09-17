using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlashCart.AuditWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
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

        // 1. Declare exchange
        await channel.ExchangeDeclareAsync(
            exchange: "flashcart.events",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // 2. Declare audit queue
        await channel.QueueDeclareAsync(
            queue: "flashcart.audit-order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // 3. Bind audit queue
        await channel.QueueBindAsync(
            queue: "flashcart.audit-order-created",
            exchange: "flashcart.events",
            routingKey: "order.created");

        // 4. Create consumer
        var consumer =
            new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body =
                    args.Body.ToArray();

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
                    "AUDIT: Order #{OrderId} was created by User #{UserId}. Total: {Total}",
                    orderCreatedEvent.OrderId,
                    orderCreatedEvent.UserId,
                    orderCreatedEvent.Total);

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process audit event.");

                await channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: true);
            }
        };

        // 5. Start consuming
        await channel.BasicConsumeAsync(
            queue: "flashcart.audit-order-created",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Audit consumer started.");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}