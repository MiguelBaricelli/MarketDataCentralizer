using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Models.BraApi;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.MarketDataBr
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class B3MarketController : ControllerBase
    {
        public readonly IDataMarketBrazilService _dataMarketBrazilService;

        public B3MarketController(IDataMarketBrazilService dataMarketBrazilService)
        {
            _dataMarketBrazilService = dataMarketBrazilService;
        }

        //[Authorize]
        [HttpGet("GetMarketDataBr/{symbol}")]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BrApiRequest>> GetMarketDataBr(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Passe o símbolo corretamente");
                }
                var brApiData = await _dataMarketBrazilService.GetAllBrApiDataAsync(symbol);

                if (brApiData == null || brApiData.BraApiResults == null || brApiData.BraApiResults.Count <= 0)
                {
                    return NotFound("Dados não encontrados");
                }
                return Ok(brApiData);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }

        //[Authorize]
        [HttpGet("GetListAssetsInfo/{symbol}")]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<BrApiModel>>> GetListAssetsInfo(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Passe o símbolo corretamente");
                }
                var brApiData = await _dataMarketBrazilService.GetListAssetsInfoAsync(symbol);

                if (brApiData == null || brApiData == null || brApiData.Count <= 0)
                {
                    return NotFound("Dados não encontrados");
                }
                return Ok(brApiData);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }

        //[Authorize]
        [HttpGet("GetRegularData/{symbol}")]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BrApiRequest), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BrApiRequest>> GetRegularDataAsync(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return BadRequest("Passe o símbolo corretamente");
                }
                var brApiData = await _dataMarketBrazilService.GetRegularDataAsset(symbol);

                if (brApiData == null)
                {
                    return NotFound("Dados não encontrados");
                }
                return Ok(brApiData);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar os dados.", ex);
            }
        }
    }
}
