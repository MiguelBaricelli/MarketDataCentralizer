using MarketDataCentralizer.Domain.Services;
using System.Text.Json.Serialization;

namespace MarketDataCentralizer.Domain.Models
{
    public class GeneralResponseModel
    {
        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<string, AlphaVantageDailyDto> WeeklyTimeSeries { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageDailyDto> TimeSeriesDaily { get; set; }

        [JsonPropertyName("Monthly Time Series")]
        public Dictionary<string, AlphaVantageDailyDto> TimeSeriesMonthly { get; set; }
    }
}
