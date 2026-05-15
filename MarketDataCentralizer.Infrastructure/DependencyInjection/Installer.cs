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
using Microsoft.Extensions.Logging;

namespace MarketDataCentralizer.Infrastructure.DependencyInjection
{
    public static class Installer
    {

        public static IServiceCollection AddDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger? logger = null)
        {
            // ================= CONSUMERS =================
            services.AddScoped<IAlphaVantageDailyConsumer, AlphaVantageDailyConsumer>();
            services.AddScoped<IAlphaVantageWeeklyConsumer, AlphaVantageWeeklyConsumer>();
            services.AddScoped<IAlphaVantageOverviewConsumer, AlphaVantageOverviewConsumer>();
            services.AddScoped<IAlphaVantageGeneralConsumer, AlphaVantageGeneralConsumer>();
            services.AddScoped<IAlphaVantageDividendsConsumer, AlphaVantageDividendsConsumer>();
            services.AddScoped<IBrApiIntegrationConsumer, BrApiIntegrationConsumer>();
            services.AddScoped<IGlobalMarketSituationConsumer, GlobalMarketSituationConsumer>();

            logger?.LogDebug("Consumers registrados.");

            // ================= REPOSITORIES =================
            services.AddScoped<IBrApiRepository, BrApiRepository>();
            services.AddScoped<IAlphaVantageRepository, AlphaVantageRepository>();

            logger?.LogDebug("Repositories registrados.");

            // ================= REDIS =================
            var connectionString = configuration.GetConnectionString("RedisConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                logger?.LogCritical(
                    "String de conexão com Redis não encontrada. " +
                    "Verifique 'ConnectionStrings:RedisConnection' no appsettings ou variáveis de ambiente.");

                throw new InvalidOperationException("String de conexão com Redis não configurada.");
            }

            try
            {
                services.AddSingleton<IRedisIntegration>(sp =>
                    new RedisIntegration(connectionString));

                services.AddScoped<ICacheRepository, RedisRepository>();

                logger?.LogInformation("Redis configurado com sucesso.");
            }
            catch (Exception ex)
            {
                logger?.LogCritical(ex,
                    "Falha ao registrar RedisIntegration. ConnectionString presente mas inválida.");
                throw;
            }

            return services;
        }
    }
}

