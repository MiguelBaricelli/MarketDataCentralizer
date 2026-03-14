using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;

namespace MarketDataCentralizer.Application.Services.Redis
{
    public class RedisTestService
    {
        private readonly ICacheRepository _cacheRepository;
        public RedisTestService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public Task<string?> TestRedis(string collection, string value, double time)
        {

            _cacheRepository.SetAsync(collection, value, TimeSpan.FromMinutes(time)).Wait();

            var result = _cacheRepository.GetAsync(collection);

            return result;
        }

        public Task<string?> GetAsync(string collection)
        {

            var result = _cacheRepository.GetAsync(collection);
            if (result == null)
            {
                return null;
            }

            return result;
        }


    }
}
