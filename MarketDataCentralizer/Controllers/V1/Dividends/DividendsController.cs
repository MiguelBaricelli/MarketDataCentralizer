using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Dividends
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DividendsController : ControllerBase
    {

        private readonly IStockDividendsService _stockDividendsService;
        public DividendsController(IStockDividendsService stockDividendsService) 
        {
            _stockDividendsService = stockDividendsService;

        }

        [HttpGet("GetDividends/{symbol}")]
        [ProducesResponseType(typeof(StockDividendResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StockDividendResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(StockDividendResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockDividendResponse>> GetDividends(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Passe o símbolo corretamente");
                }
                var response = _stockDividendsService.GetDividendResponseAsync(symbol).Result;
                if(response == null)
                {
                   return NotFound("Nenhum dado foi encontrados");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }
    }
}
