using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Dtos.DtosInputEmail
{
    public class SendEmailModel
    {
        public required string CorrelationId { get; set; }
        public string Asset { get; set; } = string.Empty;
        public required DateTime Date { get; set; }
        public required string ToEmail { get; set; }
        public required string Subject { get; set; } = string.Empty;
        public required string Message { get; set; } = string.Empty;
        public List<Dictionary<string, string>> Parameters { get; set; } 

    }
}
