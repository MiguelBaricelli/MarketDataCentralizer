using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.BraApi
{
    public class BrApiRequest
    {
        [JsonPropertyName("results")]
        public required List<BrApiModel> BraApiResults{ get; set; }
        [JsonPropertyName("requestedAt")]
        public required DateTime RequestedAt { get; set; }
    }
}
