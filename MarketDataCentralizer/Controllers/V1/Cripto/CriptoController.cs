using MarketDataCentralizer.Domain.Interfaces.Cripto;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace MarketDataCentralizer.Controllers.V1.Cripto
{
    [ApiController]
    [Route("api/v1/cripto")]
    public class CriptoController : ControllerBase
    {

        private readonly ICriptoConsumeService _criptoService;
        private readonly ILogger<CriptoController> _logger;
        public CriptoController(ICriptoConsumeService criptoService, ILogger<CriptoController> logger)
        {
            _criptoService = criptoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCriptoDataAsync([FromQuery]List<string> symbols)
        {
            try
            {
                var result = await _criptoService.GetCriptoDataAsync(symbols);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            } catch(Exception ex)
            {
                _logger.LogError("[{Class}] [{Method}] Erro ao buscar dados de cripto {Ex}",
                    nameof(CriptoController),
                    nameof(GetCriptoDataAsync), 
                    ex.Message);
                throw;
            }
            
        }
    }
}
