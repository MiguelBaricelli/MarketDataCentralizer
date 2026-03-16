using MarketDataCentralizer.Domain.Models;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IAlphaVantageWeeklyConsumer
    {
        Task<WeeklyTimeSeriesModel> TimeSeriesWeeklyConsumer(string symbol);
    }
}
