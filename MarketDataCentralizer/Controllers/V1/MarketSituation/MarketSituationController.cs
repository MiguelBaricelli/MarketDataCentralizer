using MarketDataCentralizer.Application.Services.MarketSituation;
using MarketDataCentralizer.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.MarketSituation
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MarketSituationController : ControllerBase
    {
        private readonly MarketSituationService _marketSituationService;
        public MarketSituationController(MarketSituationService marketSituationService)
        {
            _marketSituationService = marketSituationService;
        }

        [HttpGet("AllMarketSituation")]
        public async Task<ActionResult<MarketSituationResponse>> GetMarketSituationAsync()
        {
            try
            {
                var result = await _marketSituationService.GetMarketSituationAsync();

                if (result == null)
                {
                    return NotFound("Dados não encontrados");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }

        [HttpGet("MarketSituationByCountry/{region}")]
        public async Task<ActionResult<List<MarketSituationInfo>>> GetMarketSituationByCountryAsync(string region)
        {
            try
            {
                var result = await _marketSituationService.GetMarketSituationFiltredByCountryAsync(region);

                if (result == null || result.Count <= 0)
                {
                    return NotFound("Dados não encontrados para a região especificada");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }

        [HttpGet("OpenMarkets")]
        public async Task<ActionResult<List<string>>> GetOpenMarketsAsync()
        {
            try
            {
                var result = await _marketSituationService.GetMarketSituationFiltredOpenAsync();

                if (result == null || result.Count <= 0)
                {
                    return NotFound("Nenhum mercado aberto encontrado no momento");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }

        }
    }
}