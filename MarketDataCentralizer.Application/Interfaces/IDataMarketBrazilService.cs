using MarketDataCentralizer.Domain.Models.BraApi;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IDataMarketBrazilService
    {
        Task<BrApiRequest> GetAllBrApiDataAsync(string symbol);
        Task<List<BrApiModel>> GetListAssetsInfoAsync(string symbol);
        Task<List<BrApiRegularModel>> GetRegularDataAsset(string symbol);
        Task<List<BrApiRegularModel>> GetRegularDataAssetTEST(string symbol);
        Task<BrApiRequest> GetRequestBrApiAsync(string symbol);
    }
}
