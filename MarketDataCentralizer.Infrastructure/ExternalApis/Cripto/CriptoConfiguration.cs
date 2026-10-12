using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.ExternalApis.Cripto
{
    public class CriptoConfiguration
    {
        public const string SectionName = "CoinGecko";

        public string BaseUrl { get; set; } = string.Empty;
        public string HeaderName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
