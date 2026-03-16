using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    public class ApiFinanceClient
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ApiFinanceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:KeyApiFinance"]
                ?? throw new ArgumentNullException(nameof(configuration), "Api key não encontrada");
        }

        public async Task<string> GetLastCloseAsync(string s)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, _apiKey);


            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
