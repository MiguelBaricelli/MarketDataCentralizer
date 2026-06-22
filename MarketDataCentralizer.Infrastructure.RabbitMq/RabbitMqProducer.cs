using MarketDataCentralizer.Infrastructure.RabbitMq.Connection;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqProducer
{
    private readonly IRabbitMqConnection _connection;
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly ExchengesExtensions _messaginsQueues;


    public RabbitMqProducer(IRabbitMqConnection connection, ILogger<RabbitMqProducer> logger, ExchengesExtensions messaginsQueues )
    {
        _connection = connection;
        _logger = logger;
        _messaginsQueues = messaginsQueues;
    }

    public async Task PublishAsync<T>(
        string routingKey,
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

            await channel.ExchangeDeclareAsync(
                exchange: _messaginsQueues.MarketDataCentralizer,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(message));

            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: _messaginsQueues.MarketDataCentralizer,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("[{Class}] [{Method}] Mensagem publicada na fila {Queue}", nameof(RabbitMqProducer), nameof(PublishAsync), routingKey);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
       
}