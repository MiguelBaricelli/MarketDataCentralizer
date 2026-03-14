using MarketDataCentralizer.Application.Dtos.DtosInputEmail;
using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using Microsoft.Extensions.Logging;

namespace MarketDataCentralizer.Application.Services.Email
{
    public class EmailExecutor : IEmailExecutor
    {
        IGenerateMessageDailyService _generateMessageDaily;
        ISendGridIntegration _sendGridIntegraion;
        ILogger<EmailExecutor> _logger;

        public EmailExecutor(IGenerateMessageDailyService generateMessageDaily,
            ISendGridIntegration sendGridIntegration,
            ILogger<EmailExecutor> logger)
        {
            _generateMessageDaily = generateMessageDaily;
            _sendGridIntegraion = sendGridIntegration;
            _logger = logger;
        }

        public async Task<bool> ExecuteEmailDailyAsync(InputEmailGenericDailyDto emailGenericDailyDto)
        {
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.Asset))
            {
                return false;
            }
            if (emailGenericDailyDto.Date == DateTime.MinValue)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.ToEmail))
            {
                return false;
            }


            var message = await _generateMessageDaily.GenerateGenericDailyMessageAsync(emailGenericDailyDto.Asset, emailGenericDailyDto.Date, emailGenericDailyDto.ToEmail);
            if (message == null)
            {
                _logger.LogError("Email message generation failed for symbol: {Symbol}, date: {Date}, toEmail: {ToEmail}", emailGenericDailyDto.Asset, emailGenericDailyDto.Date, emailGenericDailyDto.ToEmail);
                return false;
            }

            bool sendEmail = await _sendGridIntegraion.SendEmailAsync(message);

            if (!sendEmail)
            {
                _logger.LogError("{Classe} Não foi possível enviar para a integracao do SendGrid", nameof(EmailExecutor));
                return false;
            }

            _logger.LogInformation("{Classe} Enviado com sucesso para a integracao do SendGrid", nameof(EmailExecutor));
            return true;

        }

        public async Task<bool> ExecuteLastTenEmailDailyAsync(InputEmailWithoutDateDailyDto emailGenericDailyDto)
        {
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.Asset))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.ToEmail))
            {
                return false;
            }


            var message = await _generateMessageDaily.GenerateLastTenGenericsDailyMessageAsync(emailGenericDailyDto.Asset, emailGenericDailyDto.ToEmail);
            if (message == null)
            {
                _logger.LogError("Email message generation failed for symbol: {Symbol}, toEmail: {ToEmail}", emailGenericDailyDto.Asset, emailGenericDailyDto.ToEmail);
                return false;
            }

            bool sendEmail = await _sendGridIntegraion.SendEmailAsync(message);

            if (!sendEmail)
            {
                _logger.LogError("{Classe} Não foi possível enviar para a integracao do SendGrid", nameof(EmailExecutor));
                return false;
            }

            _logger.LogInformation("{Classe} Enviado com sucesso para a integracao do SendGrid", nameof(EmailExecutor));
            return true;

        }


        public async Task<bool> ExecuteEmailVarianceDailyAsync(InputEmailGenericDailyDto emailGenericDailyDto)
        {
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.Asset))
            {
                return false;
            }
            if (emailGenericDailyDto.Date == DateTime.MinValue)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(emailGenericDailyDto.ToEmail))
            {
                return false;
            }


            var message = await _generateMessageDaily.GenerateDailyVarianceMessageAsync(emailGenericDailyDto.Asset, emailGenericDailyDto.Date, emailGenericDailyDto.ToEmail);
            if (message == null)
            {
                _logger.LogError("Email message generation failed for symbol: {Symbol}, date: {Date}, toEmail: {ToEmail}", emailGenericDailyDto.Asset, emailGenericDailyDto.Date, emailGenericDailyDto.ToEmail);
                return false;
            }

            bool sendEmail = await _sendGridIntegraion.SendEmailAsync(message);

            if (!sendEmail)
            {
                _logger.LogError("{Classe} Não foi possível enviar para a integracao do SendGrid", nameof(EmailExecutor));
                return false;
            }

            _logger.LogInformation("{Classe} Enviado com sucesso para a integracao do SendGrid", nameof(EmailExecutor));
            return true;

        }
    }
}

