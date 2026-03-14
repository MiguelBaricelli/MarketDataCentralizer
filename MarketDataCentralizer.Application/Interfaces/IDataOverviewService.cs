using MarketDataCentralizer.Application.Dtos;
using MarketDataCentralizer.Domain.Models;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IDataOverviewService
    {
        Task<OverviewModel> GetAllDataOverviewBySymbolServiceAsync(string symbol);
        Task<SummaryCompanyOverviewDto> GetCompanyOverviewSummaryServiceAsync(string symbol);
    }
}
