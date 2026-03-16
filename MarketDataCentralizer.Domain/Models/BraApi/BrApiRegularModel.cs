using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.BraApi
{
    public class BrApiRegularModel
    {
        public  string? symbol { get; set; }
        public  string? shortName { get; set; }
        public  string? longName { get; set; }
        public  string? currency { get; set; }
        public  decimal? regularMarketPrice { get; set; }
        public  decimal? regularMarketDayHigh { get; set; }
        public decimal? regularMarketDayLow { get; set; }
        public string? regularMarketDayRange { get; set; }
        public decimal? regularMarketChange { get; set; }
        public decimal? regularMarketChangePercent { get; set; }
        public string? regularMarketTime { get; set; }
        public  long? marketCap { get; set; }
        public int? regularMarketVolume { get; set; }
        public decimal? regularMarketPreviousClose { get; set; }
        public decimal? regularMarketOpen { get; set; }
        public string? fiftyTwoWeekRange { get; set; }
    }
}
