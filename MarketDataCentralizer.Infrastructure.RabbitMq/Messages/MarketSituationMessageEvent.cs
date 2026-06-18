using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.RabbitMq.Messages
{
    public class MarketSituationMessageEvent
    {
        public string Market_Type { get; set; }
        public string Region { get; set; }
        public string Primary_Exchanges { get; set; }
        public string Local_Open { get; set; }
        public string Local_Close { get; set; }
        public string Current_Status { get; set; }
        public string Notes { get; set; }
    }
}
