using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Services.Dividends
{
    public class StockDividendsService : IStockDividendsService
    {

        private readonly IAlphaVantageRepository _alphaVantageRepository;

        public StockDividendsService(IAlphaVantageRepository alphaVantageRepository)
        {
            _alphaVantageRepository = alphaVantageRepository;
        }

        public async Task<StockDividendResponse> GetDividendResponseAsync(string symbol)
        {
            return await _alphaVantageRepository.GetDividendResponseAsync(symbol);
        }
    }
}
