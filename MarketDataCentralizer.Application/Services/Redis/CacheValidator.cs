using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class CacheValidator : ICacheValidator
{
    private readonly ICacheRepository _cacheRepository;
    private readonly ILogger<CacheValidator> _logger;

    public CacheValidator(ICacheRepository cacheRepository, ILogger<CacheValidator> logger)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
    }

    private async Task<T> ExecuteCacheAsync<T>(string cacheKey, Func<Task<T>> fetchData, int time) where T : class
    {
        if (string.IsNullOrEmpty(cacheKey))
            return default;

        var isCached = await _cacheRepository.GetAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(isCached))
        {
            _logger.LogInformation("[{Class}] [{Method}] Desserializando dados do cache. Chave: {Key}",
                nameof(CacheValidator), nameof(ExecuteCacheAsync), cacheKey);

            return JsonSerializer.Deserialize<T>(isCached);
        }

        var response = await fetchData();
        if (response == null)
            return default;

        _logger.LogInformation("");
        var json = JsonSerializer.Serialize(response);

        _logger.LogInformation("[{Class}] [{Method}] SET - Armazenando no cache. Chave: {Key} / Tempo: {Time}s",
        nameof(CacheValidator), nameof(ExecuteCacheAsync), cacheKey, time);

        await _cacheRepository.SetAsync(cacheKey, json, TimeSpan.FromSeconds(time));

        return response;
    }

    public Task<T> CacheValidatorAsync<T>(string prefix, Func<Task<T>> fetchData) where T : class
        => ExecuteCacheAsync(prefix, fetchData, 120);

    public Task<T> CacheValidatorWithTimeAsync<T>(string prefix, Func<Task<T>> fetchData, int time) where T : class
        => ExecuteCacheAsync(prefix, fetchData, time);

    public Task<T> CacheValidatorWithSymbolAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData) where T : class
        => ExecuteCacheAsync($"{prefixKey}-{symbol}", fetchData, 120);

    public Task<T> CacheValidatorWithSymbolAndTimeAsync<T>(string symbol, string prefixKey, Func<Task<T>> fetchData, int time) where T : class
        => ExecuteCacheAsync($"{prefixKey}-{symbol}", fetchData, time);
}