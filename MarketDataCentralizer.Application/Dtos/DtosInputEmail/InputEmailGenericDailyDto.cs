using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Dtos.DtosInputEmail
{
    public class InputEmailGenericDailyDto
    {
        public string Asset { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ToEmail { get; set; } = string.Empty;
    }
}
