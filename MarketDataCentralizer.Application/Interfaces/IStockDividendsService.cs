using MarketDataCentralizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IStockDividendsService
    {
        Task<StockDividendResponse> GetDividendResponseAsync(string symbol);
    }
}
