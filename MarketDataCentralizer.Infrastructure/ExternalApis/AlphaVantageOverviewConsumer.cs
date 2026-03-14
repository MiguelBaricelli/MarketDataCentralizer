using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    public class AlphaVantageOverviewConsumer : IAlphaVantageOverviewConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AlphaVantageOverviewConsumer(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:AlphaVantage"]
                ?? throw new Exception("API Key Alpha Vantage não configurada");
        }

        public async Task<OverviewModel> OverviewConsumer(string symbol)
        {
            var url =
                $"https://www.alphavantage.co/query" +
                $"?function=OVERVIEW" +
                $"&symbol={symbol}" +
                $"&apikey={_apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Api não foi encontrada");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("Você não está autorizado para utilizar esse serviço");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception("Todos os campos precisam estar corretos");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("Você não é autorizado para utilizar esse serviço");
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            if (json.Contains("Error Message"))
                throw new Exception("Ativo inválido ou não encontrado");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var overview =
                JsonSerializer.Deserialize<OverviewModel>(json, options)
                ?? throw new Exception("Resposta inválida da Alpha Vantage");

            return overview;
        }
    }
}