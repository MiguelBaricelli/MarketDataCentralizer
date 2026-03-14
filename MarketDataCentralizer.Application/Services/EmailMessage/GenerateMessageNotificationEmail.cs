using MarketDataCentralizer.Application.Dtos.DtosInputEmail;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models.Email;
using Microsoft.Extensions.Logging;

namespace MarketDataCentralizer.Application.Services.EmailMessage
{
    public class GenerateMessageNotificationEmail
    {
        ILogger<GenerateMessageNotificationEmail> _logger;
        private readonly ISendGridIntegration _sendGridIntegration;
        public GenerateMessageNotificationEmail(ILogger<GenerateMessageNotificationEmail> logger,
            ISendGridIntegration sendGridIntegration)
        {
            _logger = logger;
            _sendGridIntegration = sendGridIntegration;
        }

        public async Task<bool> BuildNotificationMessage(SendEmailModel sendEmailModel)
        {
            if (sendEmailModel.Date == DateTime.MinValue)
            {
                _logger.LogError("Data inválida para geração de mensagem de notificação.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(sendEmailModel.Message))
            {
                _logger.LogError("Mensagem vazia para geração de mensagem de notificação.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(sendEmailModel.ToEmail))
            {
                _logger.LogError("Email de destino inválido para geração de mensagem de notificação.");
                return false;
            }

            var finalMessage = sendEmailModel.Message;

            if (sendEmailModel.Parameters.Count > 0)
            {
                foreach (var param in sendEmailModel.Parameters)
                {
                    foreach (var key in param.Keys)
                    {
                        var placeholder = $"{{{key}}}";
                        finalMessage = finalMessage.Replace(placeholder, param[key]);
                    }
                }
            }

            var newMessage = sendEmailModel.Message = finalMessage;

            var emailMessage = new EmailModel
            {
                Asset = sendEmailModel.Asset,
                Date = sendEmailModel.Date,
                ToEmail = sendEmailModel.ToEmail,
                Content = newMessage,
                Subject = sendEmailModel.Subject

            };

            //Colocar um Banco de dados MongoDB para logar todas as mensagens que chegar junto com o correlationId para rastreamento de cada mensagem enviada.

            var sendEmail = await _sendGridIntegration.SendEmailAsync(emailMessage).ConfigureAwait(false);

            if (!sendEmail)
            {
                return false;
                throw new Exception("Não foi possivel enviar email");

            }

            return true;
        }

    }
}
