using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models.BraApi;

namespace MarketDataCentralizer.Application.Services.DataMarketBrazil
{
    public class DataMarketBrazilService : IDataMarketBrazilService
    {
        public readonly IBrApiRepository _brApiRepository;
        public readonly ICacheRepository _cacheRepository;
        public readonly ICacheValidator _cacheValidator;
        public DataMarketBrazilService(IBrApiRepository brApiRepository, ICacheRepository cacheRepository, ICacheValidator cacheValidator)
        {
            _brApiRepository = brApiRepository;
            _cacheRepository = cacheRepository;
            _cacheValidator = cacheValidator;
        }

        public async Task<BrApiRequest> GetAllBrApiDataAsync(string symbol)
        {
            var response = await _cacheValidator.CacheValidatorAsync(symbol, () => _brApiRepository.GetBrApiDataAsync(symbol));

            return response;
        }

        public async Task<List<BrApiModel>> GetListAssetsInfoAsync(string symbol)
        {
            var response = await _cacheValidator.CacheValidatorAsync(symbol, () => _brApiRepository.GetBrApiDataAsync(symbol));

            return response.BraApiResults;
        }

        //MINHA LÓGICA COM USO DE FOR ITERANDO PELA POSIÇÃO DO ARRAY (PEGAR UM OU MAIS DE UM ATIVO E RETORNAR DADOS APENAS DO CONTRATO)
        public async Task<List<BrApiRegularModel>> GetRegularDataAsset(string symbol)
        {

            var response = await _cacheValidator.CacheValidatorAsync(symbol, () => _brApiRepository.GetBrApiDataAsync(symbol));

            var listMessage = new List<BrApiRegularModel>();

            for (var i = 0; i < response.BraApiResults.Count; i++)
            {
                var responseMessage = new BrApiRegularModel
                {
                    symbol = response.BraApiResults[i].symbol,
                    shortName = response.BraApiResults[i].shortName,
                    longName = response.BraApiResults[i].longName,
                    currency = response.BraApiResults[i].currency,
                    regularMarketPrice = response.BraApiResults[i].regularMarketPrice,
                    regularMarketDayHigh = response.BraApiResults[i].regularMarketDayHigh,
                    regularMarketDayLow = response.BraApiResults[i].regularMarketDayLow,
                    regularMarketDayRange = response.BraApiResults[i].regularMarketDayRange,
                    regularMarketChange = response.BraApiResults[i].regularMarketChange,
                    regularMarketChangePercent = response.BraApiResults[i].regularMarketChangePercent,
                    regularMarketTime = response.BraApiResults[i].regularMarketTime,
                    marketCap = response.BraApiResults[i].marketCap,
                    regularMarketVolume = response.BraApiResults[i].regularMarketVolume,
                    regularMarketPreviousClose = response.BraApiResults[i].regularMarketPreviousClose,
                    regularMarketOpen = response.BraApiResults[i].regularMarketOpen,
                    fiftyTwoWeekRange = response.BraApiResults[i].fiftyTwoWeekRange
                };

                listMessage.Add(responseMessage);
            }
            return listMessage.ToList();
        }

        //IA, UTILIZA LINQ .SELECT PARA CONSULTA DE LISTA.
        public async Task<List<BrApiRegularModel>> GetRegularDataAssetTEST(string symbol)
        {

            var response = await _cacheValidator.CacheValidatorAsync(symbol, () => _brApiRepository.GetBrApiDataAsync(symbol));

            var resultList = response.BraApiResults
                .Select(result => new BrApiRegularModel
                {
                    symbol = result.symbol,
                    shortName = result.shortName,
                    longName = result.longName,
                    currency = result.currency,
                    regularMarketPrice = result.regularMarketPrice,
                    regularMarketDayHigh = result.regularMarketDayHigh,
                    regularMarketDayLow = result.regularMarketDayLow,
                    regularMarketDayRange = result.regularMarketDayRange,
                    regularMarketChange = result.regularMarketChange,
                    regularMarketChangePercent = result.regularMarketChangePercent,
                    regularMarketTime = result.regularMarketTime,
                    marketCap = result.marketCap,
                    regularMarketVolume = result.regularMarketVolume,
                    regularMarketPreviousClose = result.regularMarketPreviousClose,
                    regularMarketOpen = result.regularMarketOpen,
                    fiftyTwoWeekRange = result.fiftyTwoWeekRange
                })
                .ToList();

            return resultList;
        }

        public async Task<BrApiRequest> GetRequestBrApiAsync(string symbol)
        {
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    return new BrApiRequest
                    {
                        BraApiResults = new List<BrApiModel>(),
                        RequestedAt = DateTime.UtcNow
                    };
                }

                var response = await _brApiRepository.GetBrApiDataAsync(symbol);

                if (response == null)
                {
                    throw new Exception("Nenhum dado encontrado");
                }

                return response;
            }
        }
    }
}
