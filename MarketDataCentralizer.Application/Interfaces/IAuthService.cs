using MarketDataCentralizer.Domain.Models.ApiClientSecurity;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IAuthService
    {

        bool ValidateApiKey(string apiKey);

        JwtTokenModel GenerateToken();

    }
}
