using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Infrastructure.ExternalApis;
using MarketDataCentralizer.Infrastructure.ExternalApis.Brapi;
using MarketDataCentralizer.Infrastructure.Redis;
using MarketDataCentralizer.Infrastructure.Repository;
using MarketDataCentralizer.Infrastructure.Repository.AlphaVantage;
using MarketDataCentralizer.Infrastructure.Repository.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketDataCentralizer.Infrastructure.DependencyInjection
{
    public static class Installer
    {

        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services, IConfiguration configuration)
        {
            // Outros serviços
            services.AddScoped<IAlphaVantageDailyConsumer, AlphaVantageDailyConsumer>();
            services.AddScoped<IAlphaVantageWeeklyConsumer, AlphaVantageWeeklyConsumer>();
            services.AddScoped<IAlphaVantageOverviewConsumer, AlphaVantageOverviewConsumer>();
            services.AddScoped<IAlphaVantageGeneralConsumer, AlphaVantageGeneralConsumer>();
            services.AddScoped<IAlphaVantageDividendsConsumer, AlphaVantageDividendsConsumer>();
            services.AddScoped<IBrApiIntegrationConsumer, BrApiIntegrationConsumer>();
            services.AddScoped<IBrApiRepository, BrApiRepository>();
            services.AddScoped<IAlphaVantageRepository, AlphaVantageRepository>();
            services.AddScoped<IGlobalMarketSituationConsumer, GlobalMarketSituationConsumer>();


            // Redis
            var connectionString = configuration.GetConnectionString("RedisConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("String de conexão com Redis não encontrada");

            // Singleton: one shared connection across the app lifetime
            services.AddSingleton<IRedisIntegration>(sp =>
                new RedisIntegration(connectionString));

            // RedisRepository now resolves IRedisIntegration cleanly — no string needed
            services.AddScoped<ICacheRepository, RedisRepository>();

            return services;
        }
    }
}

