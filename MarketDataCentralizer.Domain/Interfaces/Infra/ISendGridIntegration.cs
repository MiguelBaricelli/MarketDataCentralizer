using MarketDataCentralizer.Domain.Models.Email;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface ISendGridIntegration
    {
        Task<bool> SendEmailAsync(EmailModel emailModel);
    }
}
