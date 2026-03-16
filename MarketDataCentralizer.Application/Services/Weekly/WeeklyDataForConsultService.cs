using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Services.Weekly
{
    public class WeeklyDataForConsultService : IWeeklyDataForConsultService
    {

        private readonly IAlphaVantageWeeklyConsumer _consumer;

        public WeeklyDataForConsultService(IAlphaVantageWeeklyConsumer consumer)
        {
            _consumer = consumer;
        }

        // Pega os dados semanais completos
        public async Task<WeeklyTimeSeriesModel> GetWeeklyDataAsync(string symbol)
        {

            if (string.IsNullOrEmpty(symbol))
            {
                throw new ArgumentNullException("Precisa conter o simbolo");
            }
            var request = await _consumer.TimeSeriesWeeklyConsumer(symbol);

            if (request == null)
            {
                throw new ArgumentNullException("Objeto não encontrado");
            }

            return request;
        }

        // Pega os dados semanais por data específica
        public async Task<WeeklyTimeSeriesModel> GetDataByWeekly(string symbol, DateTime date)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException(nameof(symbol), "Precisa conter o símbolo");

            var request = await _consumer.TimeSeriesWeeklyConsumer(symbol);

            if (request == null)
                throw new Exception("Não foi possível acessar os dados");

            string dateKey = date.ToString("yyyy-MM-dd");

            if (!request.WeeklyTimeSeries.TryGetValue(dateKey, out var weeklyData))
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
            var request = await _consumer.TimeSeriesWeeklyConsumer(symbol);

            if (request == null)
                throw new Exception("Não foi possível acessar os dados");

            var result = request.WeeklyTimeSeries
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
