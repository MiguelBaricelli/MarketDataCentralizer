using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.Repository.AlphaVantage
{
    public class AlphaVantageRepository : IAlphaVantageRepository
    {

        private readonly IAlphaVantageDailyConsumer _alphaVantageDailyConsumer;
        private readonly IAlphaVantageOverviewConsumer _alphaVantageOverviewConsumer;
        private readonly IAlphaVantageWeeklyConsumer _alphaVantageWeeklyConsumer;
        private readonly IAlphaVantageGeneralConsumer _alphaVantageGeneralConsumer;
        private readonly IAlphaVantageDividendsConsumer _alphaVantageDividendsConsumer;

        public AlphaVantageRepository(IAlphaVantageDailyConsumer alphaVantageDailyConsumer,
            IAlphaVantageOverviewConsumer alphaVantageOverviewConsumer,
            IAlphaVantageWeeklyConsumer alphaVantageWeeklyConsumer,
            IAlphaVantageGeneralConsumer alphaVantageGeneralConsumer,
            IAlphaVantageDividendsConsumer alphaVantageDividendsConsumer
            )
        {
            _alphaVantageDailyConsumer = alphaVantageDailyConsumer;
            _alphaVantageOverviewConsumer = alphaVantageOverviewConsumer;
            _alphaVantageWeeklyConsumer = alphaVantageWeeklyConsumer;
            _alphaVantageGeneralConsumer = alphaVantageGeneralConsumer;
            _alphaVantageDividendsConsumer = alphaVantageDividendsConsumer;
        }

        public async Task<DailyTimeSeriesModel> GetAlphaVantageDailyDataAsync(string symbol)
        {
            return await _alphaVantageDailyConsumer.TimeSeriesDailyConsumer(symbol);
        }

        public async Task<OverviewModel> GetAlphaVantageOverviewDataAsync(string symbol)
        {
            return await _alphaVantageOverviewConsumer.OverviewConsumer(symbol);
        }

        public async Task<WeeklyTimeSeriesModel> GetAlphaVantageWeeklyDataAsync(string symbol)
        {
            return await _alphaVantageWeeklyConsumer.TimeSeriesWeeklyConsumer(symbol);
        }

        public async Task<GeneralResponseModel> GetAlphaVantageGeneralDataAsync(string symbol, FunctionAlphaVantageEnum functionAlphaVantageEnum)
        {
            return await _alphaVantageGeneralConsumer.TimeSeriesGeneralConsumer(symbol, functionAlphaVantageEnum);
        }
        public async Task<StockDividendResponse> GetDividendResponseAsync(string symbol)
        {
            return await _alphaVantageDividendsConsumer.DividendsConsumer(symbol);
        }
    }
}
