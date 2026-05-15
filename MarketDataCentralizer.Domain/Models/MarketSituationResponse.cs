using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models
{
    public class MarketSituationResponse
    {
        public List<MarketSituationInfo> Markets { get; set; }
    }

    public class MarketSituationInfo
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
