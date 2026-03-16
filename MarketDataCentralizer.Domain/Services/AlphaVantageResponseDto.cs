using System.Text.Json.Serialization;

namespace MarketDataCentralizer.Domain.Services
{
    public class AlphaVantageResponseDto
    {
        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageDailyDto> DailyTimeSeries { get; set; }
    }
}
