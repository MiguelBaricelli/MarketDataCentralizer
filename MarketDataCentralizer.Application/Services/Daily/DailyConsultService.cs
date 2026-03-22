using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Services.Daily
{
    public class DailyConsultService : IDailyConsultService
    {

        private readonly IAlphaVantageDailyConsumer _vantageDailyConsumer;
        private readonly ICacheValidator _cacheValidator;

        public DailyConsultService(IAlphaVantageDailyConsumer vantageDailyConsumer, ICacheValidator cacheValidator)
        {
            _vantageDailyConsumer = vantageDailyConsumer;
            _cacheValidator = cacheValidator;
        }

        public async Task<Dictionary<string, AlphaVantageDailyDto>> GetAllDailys(string symbol)
        {

            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("O símbolo não pode ser nulo ou vazio.", nameof(symbol));
            }

            var prefixkey = "dailyData";
            var isCache = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixkey, () => _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol)).ConfigureAwait(false);

            if (isCache == null || isCache.TimeSeriesDaily == null)
            {
                throw new Exception("Resposta inválida do consumidor Alpha Vantage.");
            }
            return isCache.TimeSeriesDaily;
        }

        public async Task<Dictionary<string, AlphaVantageDailyDto>> GetLastTenDailys(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("O símbolo não pode ser nulo ou vazio.", nameof(symbol));
            }
            var prefixkey = "dailyData";
            var isCache = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixkey, () => _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol)).ConfigureAwait(false);

            // Ordena as entradas por data (chave do dicionário) em ordem decrescente e pega as 10 mais recentes
            var lastTenDailys = isCache.TimeSeriesDaily
                .OrderByDescending(entry => DateTime.Parse(entry.Key))
                .Take(10)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            if (lastTenDailys == null || lastTenDailys.Count == 0)
            {
                throw new Exception("Resposta inválida do consumidor Alpha Vantage.");
            }

            return lastTenDailys;
        }

        public async Task<Dictionary<string, AlphaVantageDailyDto>> GetLastTwentyDailys(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("O símbolo não pode ser nulo ou vazio.", nameof(symbol));
            }
            var prefixkey = "dailyData";
            var isCache = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixkey, () => _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol)).ConfigureAwait(false);

            // Ordena as entradas por data (chave do dicionário) em ordem decrescente e pega as 10 mais recentes
            var lastTwentyDailys = isCache.TimeSeriesDaily
                .OrderByDescending(entry => DateTime.Parse(entry.Key))
                .Take(20)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            if (lastTwentyDailys == null || lastTwentyDailys.Count == 0)
            {
                throw new Exception("Resposta inválida do consumidor Alpha Vantage.");
            }

            return lastTwentyDailys;
        }
    }
}
