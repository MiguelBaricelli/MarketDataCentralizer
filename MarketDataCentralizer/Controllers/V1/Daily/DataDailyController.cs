using MarketDataCentralizer.Application.Dtos;
using MarketDataCentralizer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Daily
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DataDailyController : ControllerBase
    {
        private readonly IFinanceSummaryVarianceService _getFinanceSummaryVarianceService;

        public DataDailyController(IFinanceSummaryVarianceService getFinanceSummaryVarianceService)
        {
            _getFinanceSummaryVarianceService = getFinanceSummaryVarianceService;
        }



        /// <summary>
        /// Calcula e retorna a variância dos preços de um ativo (open, high, low, close).
        /// Importante: o parâmetro "ativo" é obrigatório.
        /// </summary>
        /// <param name="ativo">Símbolo do ativo (ex.: MSFT, AAPL, IBM)</param>
        /// <returns>Objeto FinanceSummaryDto com variância e status do ativo</returns>
        [Authorize]
        [HttpGet("GetVariationAsset/{ativo}/{date}")]
        [ProducesResponseType(typeof(FinanceSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FinanceSummaryDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(FinanceSummaryDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinanceSummaryDto>> GetFinanceSummaryVarianceController(string ativo, DateTime date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ativo))
                {
                    return BadRequest("Passe o ativo corretamente");
                }

                var summary = await _getFinanceSummaryVarianceService.GetFinanceSummaryVarianceAsync(ativo, date);

                if (summary == null)
                {
                    return NotFound("Dados não encontrados");
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }
    }
}
