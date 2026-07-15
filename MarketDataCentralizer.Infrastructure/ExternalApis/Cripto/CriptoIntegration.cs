using MarketDataCentralizer.Domain.Models.Cripto;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MarketDataCentralizer.Infrastructure.ExternalApis.Cripto
{
    public class CriptoIntegration
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CriptoIntegration> _logger;

        public CriptoIntegration(HttpClient httpclient,
            ILogger<CriptoIntegration> logger
            )
        {
            _httpClient = httpclient;
            _logger = logger;
        }

        public async Task<ResponseCriptoModel> ResultCriptoPriceAsync()
        {
            try
            {
                var url = $"https://pro-api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum&vs_currencies=usd,brl";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao consultar a API de Criptoativos. Status: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data =
                    JsonSerializer.Deserialize<ResponseCriptoModel>(json, options)
                    ?? throw new Exception("Resposta inválida da Brapi");

                if (data == null)
                {
                    Console.WriteLine("Dados do json vieram nulos ou vazios");
                    throw new Exception("Dados do json vieram nulos ou vazios");
                }

                return data;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dados de cripto");
                throw new Exception(ex.Message);
            }
        }
    }
}
