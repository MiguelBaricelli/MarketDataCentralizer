using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MarketDataCentralizer.Application.Services.Redis
{
    public class CacheValidator : ICacheValidator
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly ILogger<CacheValidator> _logger;

        public CacheValidator(ICacheRepository cacheRepository, ILogger<CacheValidator> logger)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public async Task<T> CacheValidatorAsync<T>(string prefix, Func<Task<T>> fetchData) where T : class
        {
            if (string.IsNullOrEmpty(prefix))
                return default;

            var isCached = await _cacheRepository.GetAsync(prefix);
            if (!string.IsNullOrWhiteSpace(isCached))
            {
                _logger.LogInformation("[{Class}] [{Method}] Desserializando dados do cache. Chave: {Key}",
                    nameof(CacheValidator), nameof(CacheValidatorAsync), prefix);

                return JsonSerializer.Deserialize<T>(isCached);
            }

            var response = await fetchData();
            if (response == null)
                return default;

            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation("[{Class}] [{Method}] SET - Armazenando no cache. Chave: {Key} / Tempo: {Time}s",
                nameof(CacheValidator), nameof(CacheValidatorAsync), prefix, 120);

            await _cacheRepository.SetAsync(prefix, json, TimeSpan.FromSeconds(120));

            return response;
        }

        public async Task<T> CacheValidatorWithTimeAsync<T>(string prefix, Func<Task<T>> fetchData, int time) where T : class
        {
            if (string.IsNullOrEmpty(prefix))
                return default;

            var isCached = await _cacheRepository.GetAsync(prefix);
            if (!string.IsNullOrWhiteSpace(isCached))
            {
                _logger.LogInformation("[{Class}] [{Method}] Desserializando dados do cache. Chave: {Key}",
                    nameof(CacheValidator), nameof(CacheValidatorWithTimeAsync), prefix);

                return JsonSerializer.Deserialize<T>(isCached);
            }

            var response = await fetchData();
            if (response == null)
                return default;

            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation("[{Class}] [{Method}] SET - Armazenando no cache. Chave: {Key} / Tempo: {Time}s",
                nameof(CacheValidator), nameof(CacheValidatorWithTimeAsync), prefix, time);

            await _cacheRepository.SetAsync(prefix, json, TimeSpan.FromSeconds(time));

            return response;
        }

        public async Task<T> CacheValidatorWithSymbolAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData) where T : class
        {
            if (string.IsNullOrEmpty(symbol))
                return default;

            string cacheKey = $"{prefixKey}:{symbol}";

            var isCached = await _cacheRepository.GetAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(isCached))
            {
                _logger.LogInformation("[{Class}] [{Method}] Desserializando dados do cache. Chave: {Key}",
                    nameof(CacheValidator), nameof(CacheValidatorWithSymbolAsync), cacheKey);

                return JsonSerializer.Deserialize<T>(isCached);
            }

            var response = await fetchData();
            if (response == null)
                return default;

            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation("[{Class}] [{Method}] SET - Armazenando no cache. Chave: {Key} / Tempo: {Time}s",
                nameof(CacheValidator), nameof(CacheValidatorWithSymbolAsync), cacheKey, 120);

            await _cacheRepository.SetAsync(cacheKey, json, TimeSpan.FromSeconds(120));

            return response;
        }

        public async Task<T> CacheValidatorWithSymbolAndTimeAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData, int time) where T : class
        {
            if (string.IsNullOrEmpty(symbol))
                return default;

            string cacheKey = $"{prefixKey}:{symbol}";

            var isCached = await _cacheRepository.GetAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(isCached))
            {
                _logger.LogInformation("[{Class}] [{Method}] Desserializando dados do cache. Chave: {Key}",
                    nameof(CacheValidator), nameof(CacheValidatorWithSymbolAndTimeAsync), cacheKey);

                return JsonSerializer.Deserialize<T>(isCached);
            }

            var response = await fetchData();
            if (response == null)
                return default;

            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation("[{Class}] [{Method}] SET - Armazenando no cache. Chave: {Key} / Tempo: {Time}s",
                nameof(CacheValidator), nameof(CacheValidatorWithSymbolAndTimeAsync), cacheKey, time);

            await _cacheRepository.SetAsync(cacheKey, json, TimeSpan.FromSeconds(time));

            return response;
        }
    }
}