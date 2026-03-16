using MarketDataCentralizer.Domain.Services;
using System.Text.Json.Serialization;

namespace MarketDataCentralizer.Domain.Models
{
    public class DailyTimeSeriesModel
    {
        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageDailyDto> TimeSeriesDaily { get; set; }
    }

}
