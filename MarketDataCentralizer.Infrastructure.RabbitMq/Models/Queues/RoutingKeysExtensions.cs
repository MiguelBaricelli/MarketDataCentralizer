using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.RabbitMq.Models.Queues
{
    public enum RoutingKey
    {
        MarketSituation
    }

    public static class RoutingKeyExtensions
    {
        public static string ToRoutingKey(this RoutingKey key) => key switch
        {
            RoutingKey.MarketSituation => "market_situation",
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };
    }
}
