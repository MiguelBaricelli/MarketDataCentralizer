using MarketDataCentralizer.Domain.Models;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IWeeklyDataForConsultService
    {
        Task<WeeklyTimeSeriesModel> GetWeeklyDataAsync(string symbol);
        Task<WeeklyTimeSeriesModel> GetDataByWeekly(string symbol, DateTime date);
        Task<WeeklyTimeSeriesModel> GetLastTenWeeklys(string symbol);
    }
}
