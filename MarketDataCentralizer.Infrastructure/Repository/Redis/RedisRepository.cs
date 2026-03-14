using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Infrastructure.Redis;
using StackExchange.Redis;

namespace MarketDataCentralizer.Infrastructure.Repository.Redis
{
    public class RedisRepository : ICacheRepository
    {
        private readonly IDatabase _db;

        public RedisRepository(IRedisIntegration redisIntegration)
        {
            _db = redisIntegration.GetDatabase();
        }
        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
            => await _db.StringSetAsync(key, value, (Expiration)expiry);

        public async Task<string?> GetAsync(string key)
            => await _db.StringGetAsync(key);

        public async Task RemoveAsync(string key)
            => await _db.KeyDeleteAsync(key);
    }
}
