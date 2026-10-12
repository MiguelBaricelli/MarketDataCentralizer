using MarketDataCentralizer.Domain.Interfaces.Cripto;
using MarketDataCentralizer.Domain.Models.Cripto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Services.Cripto
{
    public class CriptoConsumeService : ICriptoConsumeService
    {
        private readonly ICriptoIntegration _criptoIntegration;
        private readonly ILogger<CriptoConsumeService> _logger;
        public CriptoConsumeService(ICriptoIntegration criptoIntegration,
            ILogger<CriptoConsumeService> logger)
        {
            _criptoIntegration = criptoIntegration;
            _logger = logger;
        }

        public async Task<ResponseCriptoModel> GetCriptoDataAsync(List<string> symbols)
        {
            try
            {
                var result = await _criptoIntegration.ResultCriptoPriceAsync(symbols);

                if (result == null)
                {
                    return null;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar dados de {symbols}: {ex.Message}");
                throw new InvalidOperationException($"Erro ao buscar dados de {symbols}: {ex.Message}", ex);

            }
        }
    }
}
