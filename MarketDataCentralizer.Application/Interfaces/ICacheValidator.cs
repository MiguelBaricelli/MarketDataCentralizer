namespace MarketDataCentralizer.Application.Interfaces
{
    public interface ICacheValidator
    {
        Task<T> CacheValidatorAsync<T>(string symbol, Func<Task<T>> fetchData) where T : class;
    }
}
