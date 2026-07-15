using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.Cripto
{
    public class ResponseCriptoModel
    {
        public required string CoinId { get; init; }
        public Dictionary<string, Dictionary<string, decimal>> Prices { get; init; } = [];
    }
}
