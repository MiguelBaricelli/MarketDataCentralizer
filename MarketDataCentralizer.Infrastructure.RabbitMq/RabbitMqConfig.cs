using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Infrastructure.RabbitMq
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string VHost { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Uri { get; set; } = string.Empty; 
    }
}
