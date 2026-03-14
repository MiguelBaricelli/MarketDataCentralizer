using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.Email
{
    public class EmailModel
    {
        public string Asset { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}
