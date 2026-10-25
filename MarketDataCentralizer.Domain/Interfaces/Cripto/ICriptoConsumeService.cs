using MarketDataCentralizer.Domain.Models.Cripto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Interfaces.Cripto
{
    public interface ICriptoConsumeService
    {
        Task<ResponseCriptoModel> GetCriptoDataAsync(List<string> symbols);
    }
}
