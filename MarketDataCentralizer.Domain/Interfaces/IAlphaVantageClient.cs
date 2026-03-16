using MarketDataCentralizer.Domain.Models;

namespace MarketDataCentralizer.Infrastructure.Interfaces
{
    public interface IAlphaVantageClient
    {
        Task<FinanceDataModel> GetFinanceDataAsync(string ativo);
    }
}
