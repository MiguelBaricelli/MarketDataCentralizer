using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models.BraApi;

namespace MarketDataCentralizer.Infrastructure.Repository
{
    public class BrApiRepository : IBrApiRepository
    {
        public readonly IBrApiIntegrationConsumer _brApiIntegrationConsumer;
        public BrApiRepository(IBrApiIntegrationConsumer brApiIntegrationConsumer)
        {
            _brApiIntegrationConsumer = brApiIntegrationConsumer;

        }

        public async Task<BrApiRequest> GetBrApiDataAsync(string symbol)
        {
            return await _brApiIntegrationConsumer.GetBrapiDataAsync(symbol);
        }
    }
}
