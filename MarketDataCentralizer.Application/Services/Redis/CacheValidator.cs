using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using System.Text.Json;


namespace MarketDataCentralizer.Application.Services.Redis
{
    public class CacheValidator : ICacheValidator
    {
        private readonly ICacheRepository _cacheRepository;
        public CacheValidator(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<T> CacheValidatorAsync<T>(string symbol, Func<Task<T>> fetchData) where T : class
        {
            if (string.IsNullOrEmpty(symbol))
                return default;

            var isCached = await _cacheRepository.GetAsync(symbol);
            if (!string.IsNullOrWhiteSpace(isCached))
                return JsonSerializer.Deserialize<T>(isCached);

            // Cache miss — executa a função passada
            var response = await fetchData();
            if (response == null)
                return default;

            var json = JsonSerializer.Serialize(response);
            await _cacheRepository.SetAsync(symbol, json, TimeSpan.FromSeconds(120));

            return response;
        }
    }
}
