using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Services.MarketSituation;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.BraApi;
using MarketDataCentralizer.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;


namespace MarketDataCentralizer.Application.DataCollector
{
    public class MarketDataCollector
    {

        private readonly MarketSituationService _marketSituationService;
        private readonly IDataMarketBrazilService _dataMarketBrazilService;
        private readonly IStockDividendsService _stockDividendsService;
        private readonly ILogger<MarketDataCollector> _logger;

        IEnumerable<string> listaAtivosEua = new List<string>
        {
            "AAPL", "MSFT", "GOOGL", "AMZN", "FB", "TSLA", "NVDA", "JPM"
        };

        IEnumerable<string> listaAtivosBr = new List<string>
        {
            "PETR4", "VALE3"
        };

        IEnumerable<MarketSituationResponse> ListUniversal = new List<MarketSituationResponse>();
        private DateTime DateInitial = new DateTime(2026, 6, 20);

        public MarketDataCollector(
            MarketSituationService marketSituationService,
            IDataMarketBrazilService dataMarketBrazilService,
            IStockDividendsService stockDividendsService,
            ILogger<MarketDataCollector> logger)
        {     
            _marketSituationService = marketSituationService;
            _dataMarketBrazilService = dataMarketBrazilService;
            _stockDividendsService = stockDividendsService;
            _logger = logger;
        }

        public async Task<List<MarketSituationInfo>> CollectSituationMarket(
        CancellationToken cancellationToken)
        {


            var marketSituation = await _marketSituationService.GetMarketSituationAsync();
            _logger?.LogInformation("Situação de mercado obtida: {Status}", marketSituation.Markets);

            var situationInfos = marketSituation.Markets.Select(market => new MarketSituationInfo
            {
                Current_Status = market.Current_Status,
                Local_Close = market.Local_Close,
                Local_Open = market.Local_Open,
                Market_Type = market.Market_Type,
                Region = market.Region,
                Primary_Exchanges = market.Primary_Exchanges,
                Notes = market.Notes
            }).ToList();

            return situationInfos;
           
        }

        public async Task<List<BrApiRequest>> CollectBRDataMarket(
        CancellationToken cancellationToken)
        {
            List<BrApiRequest> listData = new List<BrApiRequest>();

            foreach(var asset in listaAtivosBr)
            {
                var data = await _dataMarketBrazilService.GetAllBrApiDataAsync(asset);

                listData.Add(data);
            }
           
            _logger?.LogInformation("Dados da B3 obtidos Qtd: {Status}", listData.Count);

            return listData;

        }

        public async Task<List<StockDividendResponse>> CollectDividendsDataMarket(
        CancellationToken cancellationToken)
        {
            List<StockDividendResponse> listData = new List<StockDividendResponse>();

            foreach (var asset in listaAtivosEua)
            {
                var data = await _stockDividendsService.GetDividendResponseAsync(asset);

                listData.Add(data);
            }

            _logger?.LogInformation("Dados da Bolsa americana sobre dividendos obtidos Qtd: {Status}", listData.Count);

            return listData;

        }
    }
}
