namespace MarketDataCentralizer.Application.Interfaces
{
    public interface ICacheValidator
    {
        Task<T> CacheValidatorAsync<T>(string symbol, Func<Task<T>> fetchData) where T : class;

        Task<T> CacheValidatorWithPrefixAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData) where T : class;
    }
}
