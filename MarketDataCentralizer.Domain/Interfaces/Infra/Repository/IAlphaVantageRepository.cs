using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Interfaces.Infra.Repository
{
    public interface IAlphaVantageRepository
    {
        Task<DailyTimeSeriesModel> GetAlphaVantageDailyDataAsync(string symbol);
        Task<OverviewModel> GetAlphaVantageOverviewDataAsync(string symbol);
        Task<WeeklyTimeSeriesModel> GetAlphaVantageWeeklyDataAsync(string symbol);
        Task<GeneralResponseModel> GetAlphaVantageGeneralDataAsync(string symbol, FunctionAlphaVantageEnum functionAlphaVantageEnum);
        Task<StockDividendResponse> GetDividendResponseAsync(string symbol);
    }
}
