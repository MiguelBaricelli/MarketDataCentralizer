namespace MarketDataCentralizer.Application.Interfaces
{
    public interface ICacheValidator
    {
        Task<T> CacheValidatorAsync<T>(string prefix, Func<Task<T>> fetchData) where T : class;
        Task<T> CacheValidatorWithTimeAsync<T>(string prefix, Func<Task<T>> fetchData, int time) where T : class;
        Task<T> CacheValidatorWithSymbolAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData) where T : class;
        Task<T> CacheValidatorWithSymbolAndTimeAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData, int time) where T : class;
    }
}
