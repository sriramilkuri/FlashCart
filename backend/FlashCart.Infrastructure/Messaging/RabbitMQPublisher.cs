using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace FlashCart.Infrastructure.Messaging;

public class RabbitMqPublisher
{
    private readonly string _connectionString;

    public RabbitMqPublisher(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task PublishAsync<T>(
        T message,
        string exchangeName,
        string routingKey)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_connectionString)
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: "flashcart.order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "flashcart.order-created",
            exchange: exchangeName,
            routingKey: routingKey);

        var json =
            JsonSerializer.Serialize(message);

        var body =
            Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            body: body);
    }
}