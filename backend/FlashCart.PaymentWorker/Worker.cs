using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlashCart.PaymentWorker;

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

        // 1. Declare the exchange
        await channel.ExchangeDeclareAsync(
            exchange: "flashcart.events",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        // 2. Declare payment queue
        await channel.QueueDeclareAsync(
            queue: "flashcart.payment-order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        // 3. Bind payment queue to exchange
        await channel.QueueBindAsync(
            queue: "flashcart.payment-order-created",
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
                    "Payment event received. OrderId: {OrderId}, UserId: {UserId}, Total: {Total}",
                    orderCreatedEvent.OrderId,
                    orderCreatedEvent.UserId,
                    orderCreatedEvent.Total);

                // Simulate payment processing
                _logger.LogInformation(
                    "Payment processing started for Order #{OrderId}",
                    orderCreatedEvent.OrderId);

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process payment event.");

                await channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: true);
            }
        };

        // 5. Start consuming
        await channel.BasicConsumeAsync(
            queue: "flashcart.payment-order-created",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Payment consumer started.");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}