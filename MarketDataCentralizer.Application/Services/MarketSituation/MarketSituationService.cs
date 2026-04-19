using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
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

        public MarketSituationService(IAlphaVantageRepository alphaVantageRepository)
        {
            _alphaVantageRepository = alphaVantageRepository;
        }

        public async Task<MarketSituationResponse> GetMarketSituationAsync()
        {
            return await _alphaVantageRepository.GetMarketSituationAsync();
        }

        public async Task<List<MarketSituationInfo>> GetMarketSituationFiltredByCountryAsync(string region)
        {
            var result = await _alphaVantageRepository.GetMarketSituationAsync();

            if(result == null || result.Markets == null || !result.Markets.Any())
            {
                return new List<MarketSituationInfo>();
            }

            var filter = result.Markets.Where(x => x.Region == region).ToList();

            return filter;

        }

        public async Task<List<MarketSituationInfo>> GetMarketSituationFiltredOpenAsync()
        {
            var now = DateTime.Now.TimeOfDay;

            var result = await _alphaVantageRepository.GetMarketSituationAsync();

            if (result == null || result.Markets == null || !result.Markets.Any())
            {
                return new List<MarketSituationInfo>();
            }

            var mercadosAbertos = result.Markets
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
                var info = result.Markets.FirstOrDefault(x => x.Region == mercado);
                if (info != null)
                {
                    listRequest.Add(info);
                }
            }

            return listRequest;

        }

    }
}