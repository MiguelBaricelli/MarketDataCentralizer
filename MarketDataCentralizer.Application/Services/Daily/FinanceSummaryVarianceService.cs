using MarketDataCentralizer.Application.Dtos;
using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Utils;
using MarketDataCentralizer.Domain.Interfaces.Infra;


namespace MarketDataCentralizer.Application.Services.Daily
{
    public class FinanceSummaryVarianceService : IFinanceSummaryVarianceService
    {
        private readonly IAlphaVantageDailyConsumer _client;
        private readonly ICacheValidator _cacheValidator;


        public FinanceSummaryVarianceService(IAlphaVantageDailyConsumer client, ICacheValidator cacheValidator)
        {
            _client = client;
            _cacheValidator = cacheValidator;
        }

        public async Task<Dictionary<string, FinanceSummaryDto>> GetFinanceSummaryVarianceAsync(string ativo, DateTime date)
        {

            if (string.IsNullOrWhiteSpace(ativo))
            {
                throw new ArgumentNullException("Ativo é obrigatorio");
            }

            var prefixKey = "financeSummaryVariance";
            var isCache = await _cacheValidator.CacheValidatorWithPrefixAsync(ativo, prefixKey, () => _client.TimeSeriesDailyConsumer(ativo)).ConfigureAwait(false);
            

            string dateKey = date.ToString("yyyy-MM-dd");

            if (!isCache.TimeSeriesDaily.TryGetValue(dateKey, out var dailyData))
                throw new Exception($"Nenhum dado foi encontrado para data {dateKey}");


            bool isAlta;
            if (dailyData.Close.ParseDecimal() > dailyData.Open.ParseDecimal())
            {

                isAlta = true;

            }
            else
            {
                isAlta = false;

            }

            var variation = AssetVariation(dailyData.Close.ParseDecimal(), dailyData.Open.ParseDecimal());

            var finnanceSummaryDto = new FinanceSummaryDto
            {

                Open = dailyData.Open.ParseDecimal(),
                High = dailyData.High.ParseDecimal(),
                Low = dailyData.Low.ParseDecimal(),
                Close = dailyData.Close.ParseDecimal(),
                Volume = dailyData.Volume.ParseDecimal(),
                Variation = variation,
                IsAlta = isAlta,
                MessageIsAlta = isAlta ? "O ativo fechou em alta" : "O ativo fechou em baixa"

            };

            return new Dictionary<string, FinanceSummaryDto>
            {

                {dateKey, finnanceSummaryDto }

            };
        }

        public decimal AssetVariation(decimal close, decimal open)
        {

            decimal variation =
             Math.Round((close - open) / open * 100, 2);

            return variation;

        }
    }
}
