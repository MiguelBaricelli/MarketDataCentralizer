using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IGeneralResponseService
    {
        Task<GeneralResponseModel> GeneralResponseServiceAsync(string symbol, DateTime date, FunctionAlphaVantageEnum vantageEnum);
        Task<GeneralResponseModel> GetGeneralData(string asset, DateTime date, FunctionAlphaVantageEnum func, int qtdNumber);
    }
}
