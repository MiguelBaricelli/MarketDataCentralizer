using MarketDataCentralizer.Application.Dtos.DtosInputEmail;

namespace MarketDataCentralizer.Application.Interfaces
{
    public interface IEmailExecutor
    {
        Task<bool> ExecuteEmailDailyAsync(InputEmailGenericDailyDto inputEmailGeneric);
        Task<bool> ExecuteLastTenEmailDailyAsync(InputEmailWithoutDateDailyDto emailGenericDailyDto);

        Task<bool> ExecuteEmailVarianceDailyAsync(InputEmailGenericDailyDto emailGenericDailyDto);
    }
}
