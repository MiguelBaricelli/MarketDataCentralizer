using MarketDataCentralizer.Application.DataCollector;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.BraApi;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace MarketDataCentralizer.Application.Messaging.Orchestrator
{
    public class MarketDataSyncOrchestrator
    {
        private readonly MarketDataCollector _collector;
        private readonly RabbitMqProducer _broker;
        private readonly ILogger<MarketDataSyncOrchestrator> _logger;

        private readonly IEnumerable<string> _symbols = new List<string>
        {
            "AAPL", "GOOGL", "MSFT"
        };

        public MarketDataSyncOrchestrator(
            MarketDataCollector collector,
            RabbitMqProducer broker,
            ILogger<MarketDataSyncOrchestrator> logger)
        {
            _collector = collector;
            _broker = broker;
            _logger = logger;
        }

        
        //Dispara TODOS os tipos em paralelo
        public async Task SyncAndPublishAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[{Class}] [{Method}] Iniciando sincronização de dados de mercado.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishAsync));

            // 1. Dispara todas as coletas em paralelo
            var situationTask = _collector.CollectSituationMarket(cancellationToken);
            var brDataTask = _collector.CollectBRDataMarket(cancellationToken);
            var dividendsTask = _collector.CollectDividendsDataMarket(cancellationToken);

            await Task.WhenAll(situationTask, brDataTask, dividendsTask)
                .ContinueWith(_ => { });

            // 2. Extrai resultados ou loga erros
            List<MarketSituationInfo> situationData = null;
            if (situationTask.IsCompletedSuccessfully)
                situationData = await situationTask;
            else
                _logger.LogError(situationTask.Exception,
                    "[{Class}] [{Method}] Falha ao coletar dados de situação de mercado.",
                    nameof(MarketDataSyncOrchestrator),
                    nameof(SyncAndPublishAsync));

            List<BrApiRequest> brData = null;
            if (brDataTask.IsCompletedSuccessfully)
                brData = await brDataTask;
            else
                _logger.LogError(brDataTask.Exception,
                    "[{Class}] [{Method}] Falha ao coletar dados da B3.",
                    nameof(MarketDataSyncOrchestrator),
                    nameof(SyncAndPublishAsync));

            List<StockDividendResponse> dividendsData = null;
            if (dividendsTask.IsCompletedSuccessfully)
                dividendsData = await dividendsTask;
            else
                _logger.LogError(dividendsTask.Exception,
                    "[{Class}] [{Method}] Falha ao coletar dados de dividendos.",
                    nameof(MarketDataSyncOrchestrator),
                    nameof(SyncAndPublishAsync));

            // 3. Publica cada conjunto com sua routing key específica
            await PublishDataAsync(situationData, RoutingKey.MarketSituation.ToRoutingKey(), cancellationToken);
            await PublishDataAsync(brData, RoutingKey.MarketBr.ToRoutingKey(), cancellationToken);
            await PublishDataAsync(dividendsData, RoutingKey.DividendsEua.ToRoutingKey(), cancellationToken);

            _logger.LogInformation(
                "[{Class}] [{Method}] Sincronização concluída (com ou sem erros parciais).",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishAsync));
        }

        
        public async Task SyncAndPublishMarketSituationAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[{Class}] [{Method}] Iniciando sincronização de MarketSituation.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishMarketSituationAsync));

            var data = await _collector.CollectSituationMarket(cancellationToken);
            await PublishDataAsync(data, RoutingKey.MarketSituation.ToRoutingKey(), cancellationToken);

            _logger.LogInformation(
                "[{Class}] [{Method}] Sincronização de MarketSituation concluída.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishMarketSituationAsync));
        }

        
        public async Task SyncAndPublishMarketBrAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[{Class}] [{Method}] Iniciando sincronização de MarketBr.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishMarketBrAsync));

            var data = await _collector.CollectBRDataMarket(cancellationToken);
            await PublishDataAsync(data, RoutingKey.MarketBr.ToRoutingKey(), cancellationToken);

            _logger.LogInformation(
                "[{Class}] [{Method}] Sincronização de MarketBr concluída.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishMarketBrAsync));
        }

       
        public async Task SyncAndPublishDividendsEuaAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[{Class}] [{Method}] Iniciando sincronização de DividendsEua.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishDividendsEuaAsync));

            var data = await _collector.CollectDividendsDataMarket(cancellationToken);
            await PublishDataAsync(data, RoutingKey.DividendsEua.ToRoutingKey(), cancellationToken);

            _logger.LogInformation(
                "[{Class}] [{Method}] Sincronização de DividendsEua concluída.",
                nameof(MarketDataSyncOrchestrator),
                nameof(SyncAndPublishDividendsEuaAsync));
        }

        //Publicação com tratamento de erro individual
        private async Task PublishDataAsync<T>(
            List<T> dataList,
            string routingKey,
            CancellationToken cancellationToken)
        {
            if (dataList == null || !dataList.Any())
            {
                _logger.LogWarning(
                    "[{Class}] [{Method}] Nenhum dado para publicar na routing key {RoutingKey}.",
                    nameof(MarketDataSyncOrchestrator),
                    nameof(PublishDataAsync),
                    routingKey);
                return;
            }

            _logger.LogInformation(
                "[{Class}] [{Method}] Publicando {Count} itens para {RoutingKey}.",
                nameof(MarketDataSyncOrchestrator),
                nameof(PublishDataAsync),
                dataList.Count,
                routingKey);

            int successCount = 0;
            int errorCount = 0;

            foreach (var item in dataList)
            {
                try
                {
                    await _broker.PublishAsync(routingKey, item, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex,
                        "[{Class}] [{Method}] Erro ao publicar mensagem na routing key {RoutingKey}. Item: {Item}",
                        nameof(MarketDataSyncOrchestrator),
                        nameof(PublishDataAsync),
                        routingKey,
                        JsonSerializer.Serialize(item));
                }
            }

            _logger.LogInformation(
                "[{Class}] [{Method}] Publicação para {RoutingKey} concluída. Sucessos: {Success}, Falhas: {Errors}.",
                nameof(MarketDataSyncOrchestrator),
                nameof(PublishDataAsync),
                routingKey,
                successCount,
                errorCount);
        }
    }
}