using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    internal class AlphaVantageGeneralConsumer : IAlphaVantageGeneralConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AlphaVantageGeneralConsumer(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:AlphaVantage"]
                ?? throw new Exception("API Key Alpha Vantage não configurada");
        }

        public async Task<GeneralResponseModel> TimeSeriesGeneralConsumer(string symbol, FunctionAlphaVantageEnum vantageEnum)
        {
            var url =
                   $"https://www.alphavantage.co/query" +
                   $"?function={vantageEnum}" +
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

            // Desserializa diretamente para o modelo que contém os dicionários (chave = data)
            var data = JsonSerializer.Deserialize<GeneralResponseModel>(json, options)
                       ?? throw new Exception("Resposta inválida da Alpha Vantage");

            // validações simples
            if ((data.TimeSeriesDaily == null || data.TimeSeriesDaily.Count == 0)
                && (data.WeeklyTimeSeries == null || data.WeeklyTimeSeries.Count == 0)
                && (data.TimeSeriesMonthly == null || data.TimeSeriesMonthly.Count == 0))
            {
                var availableProps = JsonDocument.Parse(json).RootElement.EnumerateObject().Select(p => p.Name);
                throw new Exception($"Nenhuma série temporal conhecida encontrada. Propriedades disponíveis: {string.Join(", ", availableProps)}");
            }

            return data;
        }
    }
}
