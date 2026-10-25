using MarketDataCentralizer.Domain.Interfaces.Cripto;
using MarketDataCentralizer.Domain.Models.Cripto;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text.Json;

namespace MarketDataCentralizer.Infrastructure.ExternalApis.Cripto
{
    public class CriptoIntegration : ICriptoIntegration
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

        public async Task<ResponseCriptoModel> ResultCriptoPriceAsync(List<string> symbols)
        {
            try
            {
                // Normaliza (minúsculas, sem espaços, sem duplicados) e codifica para a URL
                var ids = string.Join(",",
                    symbols
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s.Trim().ToLowerInvariant())
                        .Distinct()
                        .Select(Uri.EscapeDataString));

                var url = $"simple/price?ids={ids}&vs_currencies=usd,brl";

                using var response = await _httpClient.GetAsync(url);


                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        $"Erro ao consultar CoinGecko. Status: {response.StatusCode}");
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

                _logger.LogInformation("[{Class}] [{Method}] Dados de cripto {Dados} obtidos com sucesso em {Data}", 
                    nameof(CriptoIntegration), nameof(ResultCriptoPriceAsync),
                    data, DateTime.Now);

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
