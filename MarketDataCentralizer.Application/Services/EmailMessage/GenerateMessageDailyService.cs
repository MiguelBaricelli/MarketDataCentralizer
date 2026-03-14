using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models.Email;

namespace MarketDataCentralizer.Application.Services.EmailMessage
{
    public class GenerateMessageDailyService : IGenerateMessageDailyService
    {
        private readonly IFinanceSummaryVarianceService _financeSummaryVarianceService;
        private readonly IDailyConsultService _dailyConsultService;

        public GenerateMessageDailyService(IAlphaVantageDailyConsumer alphaVantageDailyConsumer,
            IFinanceSummaryVarianceService financeSummaryVarianceService,
            IDailyConsultService dailyConsultService
            )
        {
            _financeSummaryVarianceService = financeSummaryVarianceService;
            _dailyConsultService = dailyConsultService;
        }

        public async Task<EmailModel> GenerateDailyVarianceMessageAsync(string symbol, DateTime date, string toEmail)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException("O símbolo é obrigatório.");
            }
            var data = await _financeSummaryVarianceService.GetFinanceSummaryVarianceAsync(symbol, date);

            string message = "O ativo {symbol} teve uma variação de {data.Variation}% hoje. ";

            return new EmailModel
            {
                Asset = symbol,
                Date = date,
                Content = message,
                ToEmail = toEmail
            };
        }

        public async Task<EmailModel> GenerateCustomDailyMessageAsync(string symbol, DateTime date, string toEmail)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException("O símbolo é obrigatório.");
            }

            var data = await _financeSummaryVarianceService.GetFinanceSummaryVarianceAsync(symbol, date);

            if (data == null)
            {
                throw new Exception("Dados financeiros não encontrados para o símbolo fornecido.");
            }

            var dateKey = date.ToString("yyyy-MM-dd");

            if (data.TryGetValue(dateKey, out var responseData))
            {
                throw new Exception("Dados financeiros não encontrados para a data fornecida.");
            }

            var message = $"O ativo {symbol} apresentou as seguintes variações hoje: " +
                $"Abertura: {responseData.Open}%" +
                $"Máxima: {responseData.High}%, " +
                $"Mínima: {responseData.Low}%, " +
                $"Fechamento: {responseData.Close}%. " +
                $"A variação total foi de {responseData.Variation}%." +
                $"Em Alta: {responseData.IsAlta}" +
                $"Tendencia: {responseData.MessageIsAlta}";


            return new EmailModel
            {
                Asset = symbol,
                ToEmail = toEmail,
                Date = date,
                Content = message
            };
        }

        public async Task<EmailModel> GenerateCustomDailyMessageByClientAsync(string nameClient, string symbol, DateTime date, string toEmail)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentNullException("O símbolo é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(date.ToString()))
            {
                throw new ArgumentNullException("A data é obrigatória.");
            }

            var data = await _financeSummaryVarianceService.GetFinanceSummaryVarianceAsync(symbol, date);

            if (data == null)
            {
                throw new Exception("Dados financeiros não encontrados para o símbolo fornecido.");
            }

            var keyDate = date.ToString("yyyy-MM-dd");

            if (!data.TryGetValue(keyDate, out var responseData))
            {
                throw new Exception("Dados financeiros não encontrados para a data fornecida.");
            }


            var message = $"Olá {nameClient}, \n" +
                $"No dia {keyDate}, " +
                $"O ativo {symbol} apresentou as seguintes variações hoje: \n" +
                $"Abertura: {responseData.Open}%, \n" +
                $"Máxima: {responseData.High}%, \n" +
                $"Mínima: {responseData.Low}%, \n" +
                $"Fechamento: {responseData.Close}%. \n" +
                $"A variação total foi de {responseData.Variation}%.\n" +
                $"Em Alta: {responseData.IsAlta}" +
                $"{responseData.MessageIsAlta}";

            return new EmailModel
            {
                Asset = symbol,
                ToEmail = toEmail,
                Date = date,
                Content = message
            };
        }

        public async Task<EmailModel> GenerateGenericDailyMessageAsync(string asset, DateTime date, string toEmail)
        {
            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new ArgumentNullException("O símbolo é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(date.ToString()))
            {
                throw new ArgumentNullException("A data é obrigatória.");
            }

            var data = await _financeSummaryVarianceService.GetFinanceSummaryVarianceAsync(asset, date);

            if (data == null)
            {
                throw new Exception("Dados financeiros não encontrados para o símbolo fornecido.");
            }

            var keyDate = date.ToString("yyyy-MM-dd");

            if (!data.TryGetValue(keyDate, out var responseData))
            {
                throw new Exception("Dados financeiros não encontrados para a data fornecida.");
            }


            var message = $"Prezado cliente, \n" +
                $"No dia {keyDate}, " +
                $"O ativo {asset} apresentou as seguintes variações hoje: \n" +
                $"Abertura: {responseData.Open}%, \n" +
                $"Máxima: {responseData.High}%, \n" +
                $"Mínima: {responseData.Low}%, \n" +
                $"Fechamento: {responseData.Close}%. \n";

            return new EmailModel
            {
                Asset = asset,
                ToEmail = toEmail,
                Date = date,
                Content = message
            };
        }

        public async Task<EmailModel> GenerateLastTenGenericsDailyMessageAsync(string symbol, string toEmail)
        {
            if (string.IsNullOrWhiteSpace(symbol) || string.IsNullOrWhiteSpace(toEmail))
                return null;

            var dataList = await _dailyConsultService
                .GetLastTenDailys(symbol)
                .ConfigureAwait(false);

            if (dataList == null || !dataList.Any())
                return null;

            var linhas = dataList
            .Select(x =>
                $"<strong>{x.Key}</strong><br/>" +
                $"Abertura: {x.Value.Open}<br/>" +
                $"Máxima: {x.Value.High}<br/>" +
                $"Mínima: {x.Value.Low}<br/>" +
                $"Fechamento: {x.Value.Close}<br/>" +
                $"Volume: {x.Value.Volume}"
            )
            .ToList();

            var message =
                "<p>Prezado Cliente,</p>" +
                "<p>Segue abaixo o detalhamento completo das últimas variações diárias do ativo solicitado:</p>" +
                string.Join("<br/><br/>", linhas) +
                "<p>Atenciosamente,<br/>Equipe Data Notification</p>";






            return new EmailModel
            {
                ToEmail = toEmail,
                Subject = $"Últimas 10 variações do ativo {symbol}",
                Content = message
            };


        }
    }
}
