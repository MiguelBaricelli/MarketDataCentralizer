using MarketDataCentralizer.Infrastructure.RabbitMq.Messages;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.RabbitMq
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RabbitMqController : ControllerBase
    {
        private readonly RabbitMqProducer _producer;

        public RabbitMqController(
            RabbitMqProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> Publish()
        {
            var evt = new MarketSituationMessageEvent
            {
               Current_Status = "Fechado",
                Local_Close = "17:00",
                Local_Open = "09:30",
                Market_Type = "Ações",
                Notes = "Mercado fechado",
                Primary_Exchanges = "B3",
                Region = "Brasil"
            };

            await _producer.PublishAsync(
                "",
                evt);

            return Ok("Mensagem enviada");
        }
    }
}
