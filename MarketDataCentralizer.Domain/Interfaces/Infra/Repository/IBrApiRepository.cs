using MarketDataCentralizer.Domain.Models.BraApi;

namespace MarketDataCentralizer.Domain.Interfaces.Infra.Repository
{
    public interface IBrApiRepository
    {
        Task<BrApiRequest> GetBrApiDataAsync(string symbol);
    }
}
