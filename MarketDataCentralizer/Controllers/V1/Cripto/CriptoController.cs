using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace MarketDataCentralizer.Controllers.V1.Cripto
{
    [ApiController]
    [Route("api/v1/cripto")]
    public class CriptoController : ControllerBase
    {


        public CriptoController()
        {
              
        }

        [HttpGet]
        public IActionResult GetCriptoDataAsync(string symbol)
        {
            var url = $"https://pro-api.coingecko.com/api/v3/simple/price";

            var queryParams = new Dictionary<string, string>
            {
                { "ids", symbol },
                { "vs_currencies", "usd" }
            };

        }
    }
}
