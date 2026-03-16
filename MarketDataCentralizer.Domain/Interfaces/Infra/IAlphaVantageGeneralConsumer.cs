using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IAlphaVantageGeneralConsumer
    {
        Task<GeneralResponseModel> TimeSeriesGeneralConsumer(string symbol, FunctionAlphaVantageEnum vantageEnum);
    }
}