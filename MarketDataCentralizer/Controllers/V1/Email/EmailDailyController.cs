

using MarketDataCentralizer.Application.Dtos.DtosInputEmail;
using MarketDataCentralizer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Email
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmailDailyController : ControllerBase
    {
        private readonly ILogger<EmailDailyController> _logger;
        private readonly IEmailExecutor _emailExecutor;

        public EmailDailyController(
            ILogger<EmailDailyController> iLogger,
            IEmailExecutor emailExecutor)

        {
            _emailExecutor = emailExecutor;
            _logger = iLogger;
        }

        [Authorize]
        [HttpPost("sendGenericEmail")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<string>> SendAllGenericsDailyEmail([FromBody] InputEmailGenericDailyDto inputEmailDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputEmailDto.Asset) ||
                    inputEmailDto.Date == DateTime.MinValue ||
                    string.IsNullOrWhiteSpace(inputEmailDto.ToEmail))
                {
                    _logger.LogError("{Class} Parâmetros inválidos para o envio do email genérico diario", nameof(SendAllGenericsDailyEmail));
                    return BadRequest("Necessário informar o ativo corretamente");
                }

                var sendEmail = await _emailExecutor.ExecuteEmailDailyAsync(inputEmailDto);

                if (!sendEmail)
                {
                    _logger.LogError("Não foi possivel executar o envio do email genérico diario");
                    return NotFound(sendEmail);
                }
                return Ok(sendEmail);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("sendLastTenDailyEmail")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SendLastTenGenericsDailyEmail([FromBody] InputEmailWithoutDateDailyDto inputEmailDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputEmailDto.Asset) ||
                    string.IsNullOrWhiteSpace(inputEmailDto.ToEmail))
                {
                    _logger.LogError("{Class} Parâmetros inválidos para o envio do email genérico diario", nameof(SendLastTenGenericsDailyEmail));
                    return BadRequest("Necessário informar o ativo corretamente");
                }

                var sendEmail = await _emailExecutor.ExecuteLastTenEmailDailyAsync(inputEmailDto);

                if (!sendEmail)
                {
                    _logger.LogError("Não foi possivel executar o envio do email genérico diario");
                    return NotFound(sendEmail);
                }
                return Ok(sendEmail);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("sendVarianceDailyEmail")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> SendVarianceDailyEmail([FromBody] InputEmailGenericDailyDto inputEmailDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputEmailDto.Asset) ||
                    inputEmailDto.Date == DateTime.MinValue ||
                    string.IsNullOrWhiteSpace(inputEmailDto.ToEmail))
                {
                    _logger.LogError("{Class} Parâmetros inválidos para o envio do email genérico diario", nameof(SendVarianceDailyEmail));
                    return BadRequest("Necessário informar o ativo corretamente");
                }

                var sendEmail = await _emailExecutor.ExecuteEmailVarianceDailyAsync(inputEmailDto);

                if (!sendEmail)
                {
                    _logger.LogError("Não foi possivel executar o envio do email genérico diario");
                    return NotFound(sendEmail);
                }
                return Ok(sendEmail);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
