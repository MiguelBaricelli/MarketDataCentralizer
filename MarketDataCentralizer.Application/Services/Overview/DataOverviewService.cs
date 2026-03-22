using MarketDataCentralizer.Application.Dtos;
using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using System.Text.Json;
namespace MarketDataCentralizer.Application.Services.Overview
{
    public class DataOverviewService : IDataOverviewService
    {

        private readonly IAlphaVantageOverviewConsumer _alphaVantageOverviewConsumer;
        private readonly ICacheRepository _cacheRepository;
        private readonly ICacheValidator _cacheValidator;


        public DataOverviewService(IAlphaVantageOverviewConsumer alphaVantageOverviewConsumer, ICacheRepository cacheRepository, ICacheValidator cacheValidator)
        {
            _alphaVantageOverviewConsumer = alphaVantageOverviewConsumer;
            _cacheRepository = cacheRepository;
            _cacheValidator = cacheValidator;
        }

        public async Task<OverviewModel> GetAllDataOverviewBySymbolServiceAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException("Ativo obrigatorio");
            }

            var IsCache = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, "overview", () => _alphaVantageOverviewConsumer.OverviewConsumer(symbol)).ConfigureAwait(false);
             
            if (IsCache == null)
            {
                throw new Exception("Nenhum dado encontrado para o ativo informado.");
            }

            return IsCache;
        }

        public async Task<SummaryCompanyOverviewDto> GetCompanyOverviewSummaryServiceAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException("Ativo obrigatorio");
            }

            var IsCache = await _cacheRepository.GetAsync(symbol).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(IsCache))
            {
                return JsonSerializer.Deserialize<SummaryCompanyOverviewDto>(IsCache);
            }

            var data = await _alphaVantageOverviewConsumer.OverviewConsumer(symbol);

            if (data == null)
            {
                throw new Exception("Nenhum dado encontrado para o ativo informado.");
            }

            var response = new SummaryCompanyOverviewDto
            {
                Symbol = data.Symbol,
                AssetType = data.AssetType,
                Name = data.Name,
                Description = data.Description,
                CIK = data.CIK,
                Currency = data.Currency,
                Country = data.Country,
                Sector = data.Sector,
                Industry = data.Industry,
                Address = data.Address,
                OfficialSite = data.OfficialSite,
                MarketCapitalization = data.MarketCapitalization,
            };

            if (response == null)
            {
                throw new Exception("Dados não encontrados.");
            }

            return response;
        }
    }
}
