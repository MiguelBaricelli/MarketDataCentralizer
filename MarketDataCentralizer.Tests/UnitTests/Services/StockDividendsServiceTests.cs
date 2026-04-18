using MarketDataCentralizer.Application.Interfaces;
using MarketDataCentralizer.Application.Services.Dividends;
using MarketDataCentralizer.Domain.Interfaces.Infra.Repository;
using MarketDataCentralizer.Domain.Models;
using Moq;

namespace MarketDataCentralizer.Tests.UnitTests.Services
{
    public class StockDividendsServiceTests
    {
        [Fact]
        public async Task GetDividendResponseAsync_DeveRetornarDividendos_QuandoCacheValidatorRetorna()
        {
            // Arrange
            var symbol = "AAPL";
            var expectedResponse = new StockDividendResponse
            {
                Symbol = symbol,
                Data = new List<DividendEntry>
        {
            new DividendEntry { ExDividendDate = "2026-03-01", Amount = "0.82" }
        }
            };

            var mockRepo = new Mock<IAlphaVantageRepository>();
            var mockCacheValidator = new Mock<ICacheValidator>();

            mockCacheValidator.Setup(c => c.CacheValidatorWithPrefixAsync(
                symbol,
                "dividends",
                It.IsAny<Func<Task<StockDividendResponse>>>()))
                .ReturnsAsync(expectedResponse);

            var service = new StockDividendsService(mockRepo.Object, mockCacheValidator.Object);

            // Act
            var result = await service.GetDividendResponseAsync(symbol);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(symbol, result.Symbol);
            Assert.Single(result.Data);
            Assert.Equal("0.82", result.Data[0].Amount);
        }

        [Fact]
        public async Task GetDividendResponseAsync_DeveRetornarNull_QuandoSymbolVazio()
        {
            // Arrange
            var mockRepo = new Mock<IAlphaVantageRepository>();
            var mockCacheValidator = new Mock<ICacheValidator>();

            mockCacheValidator.Setup(c => c.CacheValidatorWithPrefixAsync(
                "", "dividends", It.IsAny<Func<Task<StockDividendResponse>>>()))
                .ReturnsAsync((StockDividendResponse)null);

            var service = new StockDividendsService(mockRepo.Object, mockCacheValidator.Object);

            // Act
            var result = await service.GetDividendResponseAsync("");

            // Assert
            Assert.Null(result);
        }
    }
}
