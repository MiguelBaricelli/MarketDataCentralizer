using MarketDataCentralizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Interfaces.Infra
{
    public interface IGlobalMarketSituationConsumer
    {
        Task<MarketSituationResponse> GetMarketSituationIntegration();
    }
}
