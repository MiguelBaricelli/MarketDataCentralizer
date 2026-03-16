using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    public class AlphaVantageDailyConsumer : IAlphaVantageDailyConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AlphaVantageDailyConsumer(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:AlphaVantage"]
                ?? throw new Exception("API Key Alpha Vantage não configurada");
        }

        public async Task<DailyTimeSeriesModel> TimeSeriesDailyConsumer(string symbol)
        {
            var url =
                   $"https://www.alphavantage.co/query" +
                   $"?function=TIME_SERIES_DAILY" +
                   $"&symbol={symbol}" +
                   $"&outputsize=compact" +
                   $"&apikey={_apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (response == null)
            {
                Console.WriteLine("Dados estão vindo nulos da api");
                throw new Exception("Dados estão vindo nulo da api");
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            if (json.Contains("Error Message"))
                throw new Exception("Ativo inválido ou não encontrado");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data =
                JsonSerializer.Deserialize<DailyTimeSeriesModel>(json, options)
                ?? throw new Exception("Resposta inválida da Alpha Vantage");

            if (data == null || data.TimeSeriesDaily == null)
            {
                Console.WriteLine("Dados do json vieram nulos ou vazios");
                throw new Exception("Dados do json vieram nulos ou vazios");
            }

            // Retorna os campos exatamente como vieram no JSON e inclui a data
            return new DailyTimeSeriesModel
            {
                TimeSeriesDaily = data.TimeSeriesDaily
            };
        }
    }
}
