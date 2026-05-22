namespace MarketDataCentralizer.Application.Interfaces
{
    public interface ICacheValidator
    {
        Task<T> CacheValidatorOnlyPrefixAsync<T>(string prefix, Func<Task<T>> fetchData, int time) where T : class;
        Task<T> CacheValidatorAsync<T>(string symbol, Func<Task<T>> fetchData) where T : class;
        Task<T> CacheValidatorWithTimeAsync<T>(string symbol, Func<Task<T>> fetchData, int time) where T : class;
        Task<T> CacheValidatorWithPrefixAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData) where T : class;
        Task<T> CacheValidatorWithPrefixAndTimeAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData, int time) where T : class;
    }
}
