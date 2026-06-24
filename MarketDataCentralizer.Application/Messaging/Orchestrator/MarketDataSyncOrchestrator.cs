using MarketDataCentralizer.Application.DataCollector;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


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
            _logger.LogInformation("{[Class]} {[Method]} Iniciando sincronização para {Count} ativos.", nameof(MarketDataSyncOrchestrator), nameof(SyncAndPublishAsync), _symbols.Count());
            var wacth = Stopwatch.StartNew();

            // Dispara todas as coletas em paralelo
            var situationTask = _collector.CollectSituationMarket(cancellationToken);
            var brDataTask = _collector.CollectBRDataMarket(cancellationToken);
            var dividendsTask = _collector.CollectDividendsDataMarket(cancellationToken);

            // Aguarda todas as tarefas concluírem
            await Task.WhenAll(situationTask, brDataTask, dividendsTask);

            var situationData = await situationTask;
            var brData = await brDataTask;
            var dividendsData = await dividendsTask;

            // 1. Publica dados de situação do mercado (EUA)
            _logger.LogInformation("{[Class]} {[Method]} Publicando {Count} dados de situação de mercado.", nameof(MarketDataSyncOrchestrator), nameof(SyncAndPublishAsync), situationData?.Count ?? 0);
            if (situationData != null && situationData.Any())
            {
                foreach (var item in situationData)
                {
                    await _broker.PublishAsync(RoutingKey.MarketSituation.ToRoutingKey(), item, cancellationToken);
                }
            }

            // 2. Publica dados da B3 (Brasil)
            _logger.LogInformation("{[Class]} {[Method]} Publicando {Count} dados da B3.", nameof(MarketDataSyncOrchestrator), nameof(SyncAndPublishAsync), brData?.Count ?? 0);
            if (brData != null && brData.Any())
            {
                foreach (var item in brData)
                {
                    // Usa routing key específica para dados da B3
                    await _broker.PublishAsync(RoutingKey.MarketSituation.ToRoutingKey(), item, cancellationToken);
                }
            }

            // 3. Publica dados de dividendos
            _logger.LogInformation("{[Class]} {[Method]} Publicando {Count} dados de dividendos.", nameof(MarketDataSyncOrchestrator), nameof(SyncAndPublishAsync), dividendsData?.Count ?? 0);
            if (dividendsData != null && dividendsData.Any())
            {
                foreach (var item in dividendsData)
                {
                    await _broker.PublishAsync(RoutingKey.MarketSituation.ToRoutingKey(), item, cancellationToken);
                }
            }

            wacth.Stop();

            _logger.LogInformation("{[Class]} {[Method]} Publicacoes concluída com sucesso. Tempo: {Time} ms", nameof(MarketDataSyncOrchestrator), nameof(SyncAndPublishAsync), wacth.ElapsedMilliseconds);
        }
    }
}
