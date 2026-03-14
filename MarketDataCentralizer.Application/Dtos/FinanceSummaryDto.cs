using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Dtos
{
    public class FinanceSummaryDto
    {
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
        public decimal Variation { get; set; } 
        public bool IsAlta { get; set; }

        public string MessageIsAlta
        {
            get; set;
        } = string.Empty;
    }
}
