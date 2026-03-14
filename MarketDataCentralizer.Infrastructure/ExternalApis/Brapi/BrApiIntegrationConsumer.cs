using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models.BraApi;
using Microsoft.Extensions.Logging;
using System.Text.Json;



namespace MarketDataCentralizer.Infrastructure.ExternalApis.Brapi
{

    public class BrApiIntegrationConsumer : IBrApiIntegrationConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrApiIntegrationConsumer> _logger;

        public BrApiIntegrationConsumer(HttpClient httpClient, ILogger<BrApiIntegrationConsumer> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BrApiRequest> GetBrapiDataAsync(string symbol)
        {
            try
            {
                var url = $"https://brapi.dev/api/quote/{symbol}";


                var response = await _httpClient.GetAsync(url);


                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Falha ao consultar {Url}. Status: {StatusCode}",
                    url, response.StatusCode);
                    throw new ExternalApiException($"Erro ao consultar ativo {symbol}. Status: {response.StatusCode}");
                }


                var json = await response.Content.ReadAsStringAsync();


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data =
                    JsonSerializer.Deserialize<BrApiRequest>(json, options)
                    ?? throw new Exception("Resposta inválida da Brapi");

                if (data == null)
                {
                    Console.WriteLine("Dados do json vieram nulos ou vazios");
                    throw new Exception("Dados do json vieram nulos ou vazios");
                }


                return data;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Erro inesperado na integração com o BrApi", e);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro de parsing JSON para ativo {Symbol}", symbol);
                throw new ExternalApiException("Erro ao processar resposta da API", ex);
            }
        }
    }
    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message, Exception? inner = null) : base(message, inner)
        { }
    }
}


