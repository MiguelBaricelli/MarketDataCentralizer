using MarketDataCentralizer.Application.Dtos;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IFinanceSummaryVarianceService
    {
        Task<Dictionary<string, FinanceSummaryDto>> GetFinanceSummaryVarianceAsync(string ativo, DateTime date);

        public decimal AssetVariation(decimal close, decimal open);
    }
}
