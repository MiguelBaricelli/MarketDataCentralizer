using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.BraApi
{
    public class BrApiModel
    {
        public required string symbol { get; set; }
        public required string shortName { get; set; }
        public required string longName { get; set; }
        public required string currency { get; set; }
        public required decimal regularMarketPrice { get; set; }
        public required decimal regularMarketDayHigh { get; set; }
        public required decimal regularMarketDayLow { get; set; }
        public required string regularMarketDayRange { get; set; }
        public required decimal regularMarketChange { get; set; }
        public required decimal regularMarketChangePercent { get; set; }
        public required string regularMarketTime { get; set; }
        public required long? marketCap { get; set; }
        public required int regularMarketVolume { get; set; }
        public required decimal regularMarketPreviousClose { get; set; }
        public required decimal regularMarketOpen { get; set; }
        public  string fiftyTwoWeekRange { get; set; } 
        public decimal fiftyTwoWeekLow { get; set; }
        public decimal fiftyTwoWeekHigh { get; set; }
        public  decimal? priceEarnings { get; set; }
        public  decimal? earningsPerShare { get; set; }
        public string logourl { get; set; }
    }
}
