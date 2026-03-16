using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Services.Daily
{
    public class DailyConsultService : IDailyConsultService
    {

        private readonly IAlphaVantageDailyConsumer _vantageDailyConsumer;

        public DailyConsultService(IAlphaVantageDailyConsumer vantageDailyConsumer)
        {
            _vantageDailyConsumer = vantageDailyConsumer;
        }

        public async Task<Dictionary<string, AlphaVantageDailyDto>> GetAllDailys(string symbol)
        {

            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("O símbolo não pode ser nulo ou vazio.", nameof(symbol));
            }
            var response = await _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol);

            if (response == null || response.TimeSeriesDaily == null)
            {
                throw new Exception("Resposta inválida do consumidor Alpha Vantage.");
            }
            return response.TimeSeriesDaily;
        }

        public async Task<Dictionary<string, AlphaVantageDailyDto>> GetLastTenDailys(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentException("O símbolo não pode ser nulo ou vazio.", nameof(symbol));
            }
            var response = await _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol);
            // Ordena as entradas por data (chave do dicionário) em ordem decrescente e pega as 10 mais recentes
            var lastTenDailys = response.TimeSeriesDaily
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
            var response = await _vantageDailyConsumer.TimeSeriesDailyConsumer(symbol);
            // Ordena as entradas por data (chave do dicionário) em ordem decrescente e pega as 10 mais recentes
            var lastTwentyDailys = response.TimeSeriesDaily
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
