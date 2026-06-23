using MarketDataCentralizer.Application.DataCollector;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues;
using Microsoft.Extensions.Logging;


namespace MarketDataCentralizer.Application.Messaging.Orchestrator
{
    public class MarketDataSyncOrchestrator
    {
        private readonly MarketDataCollector _collector;
        private readonly RabbitMqProducer _broker;
        private readonly ILogger<MarketDataSyncOrchestrator> _logger;

        // Lista de ativos que serão coletados (pode vir de configuração futuramente)
        private readonly IEnumerable<string> _symbols = new List<string>
    {
        "AAPL", "GOOGL", "MSFT"
    };

        // Construtor com injeção das dependências
        public MarketDataSyncOrchestrator(
            MarketDataCollector collector,
            RabbitMqProducer broker,
            ILogger<MarketDataSyncOrchestrator> logger)
        {
            _collector = collector;
            _broker = broker;
            _logger = logger;
        }

        public async Task SyncAndPublishAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando sincronização para {Count} ativos.", _symbols.Count());

            // 1. Coleta os dados (passando a lista de símbolos e o token)
            var rawData = await _collector.CollectSituationMarket(
                cancellationToken);

            if (rawData == null || !rawData.Any())
            {
                _logger.LogWarning("Nenhum dado retornado pelo coletor.");
                return;
            }


            foreach (var item in rawData)
            {
                await _broker.PublishAsync(
                    "market_situation",
                    item,
                    cancellationToken);
            }

            _logger.LogInformation("Publicados {Count} itens com sucesso.", rawData.Count());
        }
    }
}
