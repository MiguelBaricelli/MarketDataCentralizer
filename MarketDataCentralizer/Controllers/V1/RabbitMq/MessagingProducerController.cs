using MarketDataCentralizer.Application.Messaging.Orchestrator;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Messages;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.RabbitMq
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessagingProducerController : ControllerBase
    {
        private readonly MarketDataSyncOrchestrator _marketDataSyncOrchestrator;

        public MessagingProducerController(
            MarketDataSyncOrchestrator marketDataSyncOrchestrator)
        {
            _marketDataSyncOrchestrator = marketDataSyncOrchestrator;
        }

        [HttpPost("run/all")]
        public async Task<IActionResult> Publish(CancellationToken cancellationToken)
        {
            await _marketDataSyncOrchestrator.SyncAndPublishAsync(cancellationToken);
            return Ok("Mensagens sendo publicadas");
            
        }
    }
}
