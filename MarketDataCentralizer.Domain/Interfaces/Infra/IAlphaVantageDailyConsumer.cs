using MarketDataCentralizer.Domain.Models;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IAlphaVantageDailyConsumer
    {
        Task<DailyTimeSeriesModel> TimeSeriesDailyConsumer(string ativo);
    }
}
