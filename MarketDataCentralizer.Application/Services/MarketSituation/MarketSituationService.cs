using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Services.MarketSituation
{
    public class MarketSituationService
    {
        private readonly IAlphaVantageRepository _alphaVantageRepository;
        private readonly ICacheValidator _cacheValidator;
        private readonly ILogger<MarketSituationService> _logger;

        private const string MarketSituationPrefixKey = "MarketSituation";
        private const int dayInSeconds = 24 * 60 * 60; 

        public MarketSituationService(IAlphaVantageRepository alphaVantageRepository, 
            ICacheValidator cacheValidator,
            ILogger<MarketSituationService> logger)
        {
            _alphaVantageRepository = alphaVantageRepository;
            _cacheValidator = cacheValidator;
            _logger = logger;
        }

        public async Task<MarketSituationResponse> GetMarketSituationAsync()
        {
            var cache = await _cacheValidator.CacheValidatorOnlyPrefixAsync(MarketSituationPrefixKey, () => _alphaVantageRepository.GetMarketSituationAsync(), dayInSeconds);
            return cache;
        }

        public async Task<List<MarketSituationInfo>> GetMarketSituationFiltredByCountryAsync(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                return null;
            }
            var cache = await _cacheValidator.CacheValidatorOnlyPrefixAsync(MarketSituationPrefixKey, () =>  _alphaVantageRepository.GetMarketSituationAsync(), dayInSeconds);

            if(cache == null || cache.Markets == null)
            {
                return new List<MarketSituationInfo>();
            }

            var filter = cache.Markets.Where(x => x.Region == region).ToList();

            return filter;

        }

        public async Task<List<MarketSituationInfo>> GetMarketSituationFiltredOpenAsync()
        {
            var now = DateTime.Now.TimeOfDay;

            var cache = await _cacheValidator.CacheValidatorOnlyPrefixAsync(MarketSituationPrefixKey, () => _alphaVantageRepository.GetMarketSituationAsync(), dayInSeconds);

            var mercadosAbertos = cache.Markets
            .Where(x =>
            {
                // converte as strings "09:30" e "16:15" para TimeSpan
                var abertura = TimeSpan.Parse(x.Local_Open);
                var fechamento = TimeSpan.Parse(x.Local_Close);

                // retorna true se o horário atual estiver dentro do intervalo
                return now >= abertura && now <= fechamento;
            })
            .Select(x => x.Region) // pega só o nome da região/pais
            .ToList();

            List<MarketSituationInfo> listRequest = new List<MarketSituationInfo>();

            foreach (var mercado in mercadosAbertos)
            {
                var info = cache.Markets.FirstOrDefault(x => x.Region == mercado);
                if (info != null)
                {
                    listRequest.Add(info);
                }
            }

            return listRequest;

        }

    }
}