using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MarketDataCentralizer.Infrastructure.ExternalApis.Email
{
    public class SendGridIntegration : ISendGridIntegration
    {
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly ILogger<SendGridIntegration> _logger;


        public SendGridIntegration(IConfiguration config, ILogger<SendGridIntegration> logger)
        {
            _config = config;
            _logger = logger;
            _apiKey = _config["ApiKeys:SENDGRID_API_KEY"]
                ?? throw new Exception("API Key SendGrid não configurada");
        }

        public async Task<bool> SendEmailAsync(EmailModel emailModel)
        {
            if (string.IsNullOrEmpty(emailModel.ToEmail))
                throw new ArgumentException("O email do destinatário não pode ser nulo ou vazio.", nameof(emailModel.ToEmail));

            var client = new SendGridClient(_apiKey);
            _logger.LogInformation("SendGridClient criado.");

            var from = new EmailAddress("datamarketnotification@gmail.com", "Data Market Notification");
            var to = new EmailAddress(emailModel.ToEmail);

            var subject = emailModel.Subject;
            var plainTextContent = emailModel.Subject;
            var htmlContent = emailModel.Content;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            _logger.LogInformation("Enviando mensagem para SendGrid para {ToEmail} com assunto {subject}...", emailModel.ToEmail, emailModel.Subject);
            var response = await client.SendEmailAsync(msg);

            //Verificar o corpo da resposta para mais detalhes
            string responseBody;
            if (response.Body is HttpContent httpContent)
            {
                responseBody = await httpContent.ReadAsStringAsync();
            }
            else
            {
                responseBody = response.Body?.ToString() ?? string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(responseBody))
                _logger.LogInformation("Response body: {ResponseBody}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Falha ao enviar email para {ToEmail}. StatusCode: {StatusCode}", emailModel.ToEmail, response.StatusCode);
                return false;
            }

            _logger.LogInformation("Email enviado com sucesso para {ToEmail}", emailModel.ToEmail);
            return true;
        }
    }
}