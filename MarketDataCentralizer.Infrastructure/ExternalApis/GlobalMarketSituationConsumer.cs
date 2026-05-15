using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    public class GlobalMarketSituationConsumer : IGlobalMarketSituationConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GlobalMarketSituationConsumer> _logger;
        public GlobalMarketSituationConsumer(HttpClient httpClient, IConfiguration configuration, ILogger<GlobalMarketSituationConsumer> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:AlphaVantage"]
                ?? throw new Exception("API Key Alpha Vantage não configurada");
            _logger = logger;
        }

        public async Task<MarketSituationResponse> GetMarketSituationIntegration()
        {
            var url = $"https://www.alphavantage.co/query?function=MARKET_STATUS&apikey={_apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (response == null)
            {
                Console.WriteLine("Dados estão vindo nulos da api");
                throw new Exception("Dados estão vindo nulo da api");
            }

            response.EnsureSuccessStatusCode();

            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                _logger.LogError($"Erro ao tentar buscar dados da situacao do mercado, retornou com erro {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            if (json.Contains("Error Message"))
                throw new Exception("Ativo inválido ou não encontrado");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data =
                JsonSerializer.Deserialize<MarketSituationResponse>(json, options)
                ?? throw new Exception("Resposta inválida dos dados de situacao de mercado");

            _logger.LogInformation($"Dados foram deserializados com sucesso: {data}");

            if (data == null || data.Markets.Count <= 0)
            {
                Console.WriteLine("Dados do json vieram nulos ou vazios");
                throw new Exception("Dados do json vieram nulos ou vazios");
            }

            // Retorna os campos exatamente como vieram no JSON e inclui a data
            return new MarketSituationResponse
            {
                Markets = data.Markets
            };
        }
    }
}
