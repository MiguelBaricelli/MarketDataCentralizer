using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Dtos.DtosInputEmail
{
    public class InputEmailWithoutDateDailyDto
    {
       
            public string Asset { get; set; } = string.Empty;
            public string ToEmail { get; set; } = string.Empty;
        }
}
