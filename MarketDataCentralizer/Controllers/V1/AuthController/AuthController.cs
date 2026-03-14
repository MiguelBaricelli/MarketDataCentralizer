using MarketDataCentralizer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.AuthController
{
    [ApiController]
    [Route("v1/auth/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromHeader] string apiKey)
        {

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return BadRequest();
            }
            if (!_authService.ValidateApiKey(apiKey))
                return Unauthorized("API Key inválida");

            var token = _authService.GenerateToken();
            return Ok(token);

        }
    }
}
