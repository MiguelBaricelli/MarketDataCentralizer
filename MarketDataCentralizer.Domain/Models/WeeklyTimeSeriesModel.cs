using MarketDataCentralizer.Domain.Services;
using System.Text.Json.Serialization;

namespace MarketDataCentralizer.Domain.Models
{
    public class WeeklyTimeSeriesModel
    {
        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<string, AlphaVantageDailyDto> WeeklyTimeSeries { get; set; }
    }
}
