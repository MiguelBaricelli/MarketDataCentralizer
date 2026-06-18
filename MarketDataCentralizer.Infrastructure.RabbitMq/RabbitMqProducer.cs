using MarketDataCentralizer.Infrastructure.RabbitMq.Connection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqProducer
{
    private readonly IRabbitMqConnection _connection;
    private readonly ILogger<RabbitMqProducer> _logger;

    public RabbitMqProducer(IRabbitMqConnection connection, ILogger<RabbitMqProducer> logger )
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        string queueName,
        T message)
    {

        try
        {
            var conn = await _connection.GetConnectionAsync();

            await using var channel = await conn.CreateChannelAsync();

            var arguments = new Dictionary<string, object?>
            {
                 { "x-message-ttl", 86400000 } 
            };

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("[{Class}] [{Method}] Mensagem publicada na fila {Queue}", nameof(RabbitMqProducer), nameof(PublishAsync), queueName);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
       
}