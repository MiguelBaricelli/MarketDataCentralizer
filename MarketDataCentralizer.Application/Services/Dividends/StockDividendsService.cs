using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Services.Dividends
{
    public class StockDividendsService : IStockDividendsService
    {

        private readonly IAlphaVantageRepository _alphaVantageRepository;
        private readonly ICacheValidator _cacheValidator;

        public StockDividendsService(IAlphaVantageRepository alphaVantageRepository,
            ICacheValidator cacheValidator)
        {
            _alphaVantageRepository = alphaVantageRepository;
            _cacheValidator = cacheValidator;
        }

        public async Task<StockDividendResponse> GetDividendResponseAsync(string symbol)
        {
            var isCached = await _cacheValidator.CacheValidatorAsync(symbol, () => _alphaVantageRepository.GetDividendResponseAsync(symbol));

            return isCached;
        }
    }
}
