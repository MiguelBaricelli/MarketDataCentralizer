using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Services.Weekly
{
    public class WeeklyDataForConsultService : IWeeklyDataForConsultService
    {

        private readonly IAlphaVantageWeeklyConsumer _consumer;
        private readonly ICacheValidator _cacheValidator;

        public WeeklyDataForConsultService(IAlphaVantageWeeklyConsumer consumer, ICacheValidator cacheValidator)
        {
            _consumer = consumer;
            _cacheValidator = cacheValidator;
        }

        // Pega os dados semanais completos
        public async Task<WeeklyTimeSeriesModel> GetWeeklyDataAsync(string symbol)
        {

            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentNullException("Precisa conter o simbolo");
            }
            var prefixKey = "weeklyData";
            var isCached = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixKey, () => _consumer.TimeSeriesWeeklyConsumer(symbol)).ConfigureAwait(false);

            if (isCached == null)
            {
                return null;
            }

            return isCached;
        }

        // Pega os dados semanais por data específica
        public async Task<WeeklyTimeSeriesModel> GetDataByWeekly(string symbol, DateTime date)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException(nameof(symbol), "Precisa conter o símbolo");

            var prefixKey = "weeklyData";
            var isCached = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixKey, () => _consumer.TimeSeriesWeeklyConsumer(symbol)).ConfigureAwait(false);

            if (isCached == null)
                return null;

            string dateKey = date.ToString("yyyy-MM-dd");

            if (!isCached.WeeklyTimeSeries.TryGetValue(dateKey, out var weeklyData))
                throw new Exception($"Nenhum dado foi encontrado para data {dateKey}");

            // Retorna apenas a semana solicitada dentro de um novo objeto
            return new WeeklyTimeSeriesModel
            {
                WeeklyTimeSeries = new Dictionary<string, AlphaVantageDailyDto>
        {
            { dateKey, weeklyData }
        }
            };
        }

        // Pega os dados semanais dos últimos 10 períodos (semanas)
        public async Task<WeeklyTimeSeriesModel> GetLastTenWeeklys(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return null;
            }

            var prefixKey = "weeklyData";
            var isCached = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixKey, () => _consumer.TimeSeriesWeeklyConsumer(symbol)).ConfigureAwait(false);

            if (isCached == null)
                return null;

            var result = isCached.WeeklyTimeSeries
            .OrderByDescending(x => x.Key) // mais recente primeiro
            .Take(10)
            .ToDictionary(x => x.Key, x => x.Value);

            return new WeeklyTimeSeriesModel
            {
                WeeklyTimeSeries = result
            };
        }

    }
}
