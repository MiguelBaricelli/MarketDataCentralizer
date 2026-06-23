using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Services.MarketSituation;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Logging;


namespace MarketDataCentralizer.Application.DataCollector
{
    public class MarketDataCollector
    {

        private readonly MarketSituationService _marketSituationService;
        private readonly ILogger<MarketDataCollector> _logger;

        IEnumerable<string> listaAtivosEua = new List<string>
        {
            "AAPL", "MSFT", "GOOGL", "AMZN", "FB", "TSLA", "NVDA", "JPM"
        };

        IEnumerable<MarketSituationResponse> Markets = new List<MarketSituationResponse>();
        private DateTime DateInitial = new DateTime(2026, 6, 20);

        public MarketDataCollector(
            MarketSituationService market,
            ILogger<MarketDataCollector> logger)
        {     
            _marketSituationService = market;
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
    }
}
