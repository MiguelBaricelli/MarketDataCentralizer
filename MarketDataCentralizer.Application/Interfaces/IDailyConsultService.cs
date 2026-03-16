using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IDailyConsultService
    {
        Task<Dictionary<string, AlphaVantageDailyDto>> GetAllDailys(string symbol);

        Task<Dictionary<string, AlphaVantageDailyDto>> GetLastTenDailys(string symbol);

        Task<Dictionary<string, AlphaVantageDailyDto>> GetLastTwentyDailys(string symbol);
    }
}
