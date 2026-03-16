using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models
{
    public class StockDividendResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("data")]
        public List<DividendEntry> Data { get; set; }
    }

    public class DividendEntry
    {
        [JsonPropertyName("ex_dividend_date")]
        public string ExDividendDate { get; set; }

        [JsonPropertyName("declaration_date")]
        public string DeclarationDate { get; set; }

        [JsonPropertyName("record_date")]
        public string RecordDate { get; set; }

        [JsonPropertyName("payment_date")]
        public string PaymentDate { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }
}
