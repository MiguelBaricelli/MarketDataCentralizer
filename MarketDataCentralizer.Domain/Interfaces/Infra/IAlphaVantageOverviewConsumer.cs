using MarketDataCentralizer.Domain.Models;
namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IAlphaVantageOverviewConsumer
    {
        Task<OverviewModel> OverviewConsumer(string symbol);
    }
}
