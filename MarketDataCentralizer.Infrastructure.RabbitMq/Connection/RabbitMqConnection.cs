using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.RabbitMq.Connection
{
    public sealed class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory _factory;
        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ILogger<RabbitMqConnection> _logger;

        public RabbitMqConnection(IConnectionFactory factory, ILogger<RabbitMqConnection> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            if (_connection != null && _connection.IsOpen)
            {
                _logger.LogInformation(
                    "[{Class}] [{Method}] RabbitMQ conectado: {Connected} Host:{Host} Port:{Port}",
                    nameof(RabbitMqConnection),
                    nameof(GetConnectionAsync),
                    _connection.IsOpen,
                    _connection.Endpoint.HostName,
                    _connection.Endpoint.Port);

                return _connection;
            }

            await _lock.WaitAsync();

            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                _connection = await _factory.CreateConnectionAsync();

                _logger.LogInformation(
                    "[{Class}] [{Method}] Conexão RabbitMQ criada com sucesso",
                    nameof(RabbitMqConnection),
                    nameof(GetConnectionAsync));

                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
 }
