using MarketDataCentralizer.Infrastructure.RabbitMq.Connection;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class RabbitMqConsumerController : ControllerBase
{
    private readonly IRabbitMqConnection _connection;
    private static readonly List<string> _messages = new(); // lista em memória

    public RabbitMqConsumerController(IRabbitMqConnection connection)
    {
        _connection = connection;
    }
    //
    [HttpGet("consume")]
    public async Task<IActionResult> Consume()
    {
        var conn = await _connection.GetConnectionAsync();
        var channel = await conn.CreateChannelAsync(); 

        await channel.QueueDeclareAsync(
            queue: "market_data",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "market_data",       
            exchange: "market_data_centralizer",
            routingKey: "market_situation");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _messages.Add(message);
            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: "market_data",
            autoAck: false,
            consumer: consumer);

        return Ok("Consumer iniciado. Mensagens serão armazenadas em memória.");
    }

    [HttpGet("messages")]
    public IActionResult GetMessages()
    {
        // Retorna a lista em memória
        return Ok(_messages);
    }
}
