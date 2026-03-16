using MarketDataCentralizer.Domain.Models.BraApi;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IBrApiIntegrationConsumer
    {
        Task<BrApiRequest> GetBrapiDataAsync(string symbol);
    }
}
