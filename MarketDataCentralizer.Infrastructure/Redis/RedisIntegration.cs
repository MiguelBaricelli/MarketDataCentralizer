using StackExchange.Redis;

namespace MarketDataCentralizer.Infrastructure.Redis
{
    public class RedisIntegration : IRedisIntegration
    {
        private readonly IConnectionMultiplexer _connection;

        public RedisIntegration(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
        }

        public IDatabase GetDatabase()
        {
            return _connection.GetDatabase();
        }
    }
}
