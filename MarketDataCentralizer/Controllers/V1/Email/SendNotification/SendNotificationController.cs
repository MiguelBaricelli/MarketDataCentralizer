using MarketDataCentralizer.Application.Dtos.DtosInputEmail;
using MarketDataCentralizer.Application.Services.EmailMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketDataCentralizer.Controllers.V1.Email.SendNotification
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SendNotificationController : ControllerBase
    {
        ILogger<SendNotificationController> _logger;
        private readonly GenerateMessageNotificationEmail _generateMessageNotificationEmail;

        public SendNotificationController(ILogger<SendNotificationController> logger,
            GenerateMessageNotificationEmail generateMessageNotificationEmail)
        {
            _logger = logger;
            _generateMessageNotificationEmail = generateMessageNotificationEmail;
        }

        [Authorize]
        [HttpPost("sendEmail")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromBody] SendEmailModel sendEmailModel)
        {
            try
            {
                _logger.LogInformation("Iniciando envio de email");

                var isSend = await _generateMessageNotificationEmail
                    .BuildNotificationMessage(sendEmailModel);

                if (!isSend)
                {
                    return BadRequest("Erro ao enviar email");
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro interno ao processar a requisição"
                );
            }
        }

    }
}
