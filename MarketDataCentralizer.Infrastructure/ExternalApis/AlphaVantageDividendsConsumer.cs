using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MarketDataCentralizer.Infrastructure.ExternalApis
{
    public class AlphaVantageDividendsConsumer : IAlphaVantageDividendsConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public AlphaVantageDividendsConsumer(HttpClient httpClient, IConfiguration configuration) 
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKeys:AlphaVantage"]
                ?? throw new Exception("API Key Alpha Vantage não configurada");
        }

        public async Task<StockDividendResponse> DividendsConsumer(string symbol)
        {
            var url = "https://www.alphavantage.co/query" +
                "?function=DIVIDENDS" +
                $"&symbol={symbol}" +
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

            
            var data = JsonSerializer.Deserialize<StockDividendResponse>(json, options)
                       ?? throw new Exception("Resposta inválida da Alpha Vantage");

            return data;

        }
    }
}
