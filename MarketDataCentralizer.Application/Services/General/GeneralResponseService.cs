using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra;
using MarketDataCentralizer.Domain.Models;
using MarketDataCentralizer.Domain.Models.Enums;
using MarketDataCentralizer.Domain.Services;

namespace MarketDataCentralizer.Application.Services.General
{
    public class GeneralResponseService : IGeneralResponseService
    {
        private readonly IAlphaVantageGeneralConsumer _alphaVantageGeneralConsumer;
        private readonly ICacheValidator _cacheValidator;
        public GeneralResponseService(IAlphaVantageGeneralConsumer alphaVantageGeneralConsumer, ICacheValidator cacheValidator)
        {
            _alphaVantageGeneralConsumer = alphaVantageGeneralConsumer;
            _cacheValidator = cacheValidator;
        }

        /// <summary>
        /// Retorna um modelo específico conforme o tipo de time series:
        /// - Daily: popula `TimeSeriesDaily` (até 100 pontos)
        /// - Weekly: popula `WeeklyTimeSeries` (30 semanas mais recentes)
        /// - Monthly: popula `TimeSeriesMonthly` (meses do ano informado)
        /// </summary>
        public async Task<GeneralResponseModel> GeneralResponseServiceAsync(string symbol, DateTime date, FunctionAlphaVantageEnum vantageEnum)
        {
            var prefixKey = $"generalResponse:{symbol}:{vantageEnum}";
            var isCache = await _cacheValidator.CacheValidatorWithPrefixAsync(symbol, prefixKey, 
                () => _alphaVantageGeneralConsumer.TimeSeriesGeneralConsumer(symbol, vantageEnum)).ConfigureAwait(false);
     
            // Escolhe a série conforme o enum solicitado; se não existir, faz fallback para a primeira série disponível
            Dictionary<string, AlphaVantageDailyDto>? sourceSeries = vantageEnum switch
            {
                FunctionAlphaVantageEnum.TIME_SERIES_DAILY => isCache.TimeSeriesDaily,
                FunctionAlphaVantageEnum.TIME_SERIES_WEEKLY => isCache.WeeklyTimeSeries,
                FunctionAlphaVantageEnum.TIME_SERIES_MONTHLY => isCache.TimeSeriesMonthly,
                _ => null
            };

            if (sourceSeries == null || sourceSeries.Count == 0)
            {
                // fallback: primeira série não-nula encontrada
                sourceSeries = isCache.TimeSeriesDaily ?? isCache.WeeklyTimeSeries ?? isCache.TimeSeriesMonthly;
            }

            if (sourceSeries == null || sourceSeries.Count == 0)
                throw new Exception("Nenhuma série temporal disponível na resposta.");

            // Envelope que será retornado com apenas a propriedade correspondente preenchida
            var general = new GeneralResponseModel();

            switch (vantageEnum)
            {
                case FunctionAlphaVantageEnum.TIME_SERIES_DAILY:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(100)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesDaily = selected;
                        return general;
                    }

                case FunctionAlphaVantageEnum.TIME_SERIES_WEEKLY:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(30)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.WeeklyTimeSeries = selected;
                        return general;
                    }

                case FunctionAlphaVantageEnum.TIME_SERIES_MONTHLY:
                    {
                        var selected = sourceSeries
                            .Where(k =>
                            {
                                if (DateTime.TryParse(k.Key, out var dt))
                                    return dt.Year == date.Year;
                                return k.Key.StartsWith(date.Year.ToString());
                            })
                            .OrderByDescending(k => k.Key)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesMonthly = selected;
                        return general;
                    }

                default:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(100)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesDaily = selected;
                        return general;
                    }
            }
        }

        public async Task<GeneralResponseModel> GetGeneralData(string asset, DateTime date, FunctionAlphaVantageEnum func, int qtdNumber)
        {
            if (qtdNumber <= 0 || qtdNumber > 100)
                qtdNumber = 100;

            var response = await _alphaVantageGeneralConsumer.TimeSeriesGeneralConsumer(asset, func);

            // Escolhe a série conforme o enum solicitado; se não existir, faz fallback para a primeira série disponível
            Dictionary<string, AlphaVantageDailyDto>? sourceSeries = func switch
            {
                FunctionAlphaVantageEnum.TIME_SERIES_DAILY => response.TimeSeriesDaily,
                FunctionAlphaVantageEnum.TIME_SERIES_WEEKLY => response.WeeklyTimeSeries,
                FunctionAlphaVantageEnum.TIME_SERIES_MONTHLY => response.TimeSeriesMonthly,
                _ => null
            };

            if (sourceSeries == null || sourceSeries.Count == 0)
            {
                // fallback: primeira série não-nula encontrada
                sourceSeries = response.TimeSeriesDaily ?? response.WeeklyTimeSeries ?? response.TimeSeriesMonthly;
            }

            if (sourceSeries == null || sourceSeries.Count == 0)
                throw new Exception("Nenhuma série temporal disponível na resposta.");


            var general = new GeneralResponseModel();

            switch (func)
            {
                case FunctionAlphaVantageEnum.TIME_SERIES_DAILY:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(qtdNumber)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesDaily = selected;
                        return general;
                    }

                case FunctionAlphaVantageEnum.TIME_SERIES_WEEKLY:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(qtdNumber)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.WeeklyTimeSeries = selected;
                        return general;
                    }

                case FunctionAlphaVantageEnum.TIME_SERIES_MONTHLY:
                    {
                        var selected = sourceSeries
                            .Where(k =>
                            {
                                if (DateTime.TryParse(k.Key, out var dt))
                                    return dt.Year == date.Year;
                                return k.Key.StartsWith(date.Year.ToString());
                            })
                            .OrderByDescending(k => k.Key)
                            .Take(qtdNumber)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesMonthly = selected;
                        return general;
                    }

                default:
                    {
                        var selected = sourceSeries
                            .OrderByDescending(k => k.Key)
                            .Take(qtdNumber)
                            .ToDictionary(k => k.Key, k => k.Value);

                        general.TimeSeriesDaily = selected;
                        return general;
                    }
            }
        }

    }
}
