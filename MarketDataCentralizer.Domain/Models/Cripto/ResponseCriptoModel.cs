using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.Cripto
{
    public class ResponseCriptoModel : Dictionary<string, CriptoPriceModel>
    {
    } 

    public class CriptoPriceModel
    {
        public decimal Usd { get; set; }

        public decimal Brl { get; set; }
    }
}
