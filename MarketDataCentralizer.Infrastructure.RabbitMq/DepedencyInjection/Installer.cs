using MarketDataCentralizer.Infrastructure.RabbitMq.Connection;
using MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MarketDataCentralizer.Infrastructure.RabbitMq.DependencyInjection;

public static class Installer
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration
            .GetSection("RabbitMQ")
            .Get<RabbitMqConfig>()
            ?? throw new InvalidOperationException(
                "Configuração RabbitMQ não encontrada.");

        services.AddSingleton(settings);

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var factory = new ConnectionFactory();

            
            if (!string.IsNullOrEmpty(settings.Uri))
            {
                factory.Uri = new Uri(settings.Uri);
            }
            else
            {
               
                factory.HostName = settings.HostName;
                factory.UserName = settings.UserName;
                factory.VirtualHost = settings.VHost;
                factory.Password = settings.Password;
                factory.Port = settings.Port;
            }

            // Configurações de recuperação (sempre boas)
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            factory.RequestedHeartbeat = TimeSpan.FromSeconds(30);

            return factory;
        });


        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddScoped<RabbitMqProducer>();

        services.AddSingleton<ExchengesExtensions>();

        return services;
    }
}