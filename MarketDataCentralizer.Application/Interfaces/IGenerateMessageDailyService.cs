using MarketDataCentralizer.Domain.Models.Email;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IGenerateMessageDailyService
    {
        Task<EmailModel> GenerateGenericDailyMessageAsync(string asset, DateTime date, string toEmail);

        Task<EmailModel> GenerateLastTenGenericsDailyMessageAsync(string symbol, string toEmail);
        Task<EmailModel> GenerateDailyVarianceMessageAsync(string asset, DateTime date, string toEmail);
        Task<EmailModel> GenerateCustomDailyMessageByClientAsync(string clientName, string asset, DateTime date, string toEmail);
    }
}
