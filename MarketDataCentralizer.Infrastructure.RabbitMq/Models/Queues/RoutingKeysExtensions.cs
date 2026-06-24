using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues
{
    public enum RoutingKey
    {
        MarketSituation,
        MarketBr,
        DividendsEua
    }

    public static class RoutingKeyExtensions
    {
        public static string ToRoutingKey(this RoutingKey key) => key switch
        {
            RoutingKey.MarketSituation => "market_situation",
            RoutingKey.MarketBr => "market_Brazil",
            RoutingKey.DividendsEua => "dividends_Eua",
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, "Valor de RoutingKey não suportado.")
        };
    }
}
