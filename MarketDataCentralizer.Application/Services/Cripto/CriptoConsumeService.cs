using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Cripto;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models.Cripto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Services.Cripto
{
    public class CriptoConsumeService : ICriptoConsumeService
    {
        private readonly ICriptoIntegration _criptoIntegration;
        public readonly ICacheValidator _cacheValidator;
        private readonly ILogger<CriptoConsumeService> _logger;

        private string prefixKey = "CriptoDefault";

        public CriptoConsumeService(ICriptoIntegration criptoIntegration,
            ILogger<CriptoConsumeService> logger,
             ICacheValidator cacheValidator)
        {
            _criptoIntegration = criptoIntegration;
            _logger = logger;
            _cacheValidator = cacheValidator;
        }

        public async Task<ResponseCriptoModel> GetCriptoDataAsync(List<string> symbols)
        {
            try
            {

                var result = await _cacheValidator.CacheValidatorAsync(prefixKey, () => _criptoIntegration.ResultCriptoPriceAsync(symbols));

                return result;
            }
            catch (Exception ex)
            {
                string symbolsStr = string.Join(", ", symbols);
                _logger.LogError(ex, $"Erro ao buscar dados de {symbolsStr}: {ex.Message}");
                throw new InvalidOperationException($"Erro ao buscar dados de {symbolsStr}: {ex.Message}", ex);

            }
        }
    }
}
