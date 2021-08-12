using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Functions;
using UrlShortener.Models.Dto;
using UrlShortener.Models.Entities;
using UrlShortener.Repository;

namespace UrlShortener.UnitTests.Functions
{
    [TestClass]
    public class GenerateShortenedUrlTests : TestBase
    {
        private readonly Mock<ICodeGenerator> _mockCodeGenerator = new Mock<ICodeGenerator>();
        private readonly Mock<ITableStorageRepository<UrlCodeTableEntity>> _mockTableStorageRepository = new Mock<ITableStorageRepository<UrlCodeTableEntity>>();

        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();

        private GenerateShortenedUrl _generateShortenedUrl;

        [TestInitialize]
        public void Initialize()
        {
            _generateShortenedUrl = new GenerateShortenedUrl(_mockCodeGenerator.Object, _mockTableStorageRepository.Object);
        }

        [TestMethod]
        public async Task GivenAFullUrl_WhenRun_ThenShortUrlAndCodeReturned()
        {
            // arrange
            var requestBody = new UrlShortenRequest
            {
                Url = "https://www.foo.co.uk/bar-bar-bar-bar-bar-bar"
            };

            var req = CreateMockRequest(requestBody);

            const string code = "abcdefgh";
            _mockCodeGenerator
                .Setup(s => s.Generate(8))
                .Returns(code);

            // act
            var response = await _generateShortenedUrl.Run(req, _mockLogger.Object);

            // assert
            var result = response as OkObjectResult;
            var responseBody = result.Value as UrlShortenResponse;

            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsTrue(responseBody.ShortUrl.Contains(code));
            Assert.AreEqual(code, responseBody.Code);

            _mockTableStorageRepository
                .Verify(v => v.InsertAsync(It.Is<UrlCodeTableEntity>(entity => entity.FullUrl == requestBody.Url)), Times.Once);
        }

        [TestMethod]
        public async Task GivenAFullUrl_WhenRun_AndExceptionThrown_ThenInternalServerErrorReturned()
        {
            // arrange
            var requestBody = new UrlShortenRequest
            {
                Url = "https://www.foo.co.uk/bar-bar-bar-bar-bar-bar"
            };

            var req = CreateMockRequest(requestBody);

            var exception = new Exception("boom");
            _mockTableStorageRepository
                .Setup(s => s.InsertAsync(It.IsAny<UrlCodeTableEntity>()))
                .Throws<Exception>();

            // act
            var response = await _generateShortenedUrl.Run(req, _mockLogger.Object);

            // assert
            var result = response as InternalServerErrorResult;

            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);

            _mockLogger
                .Verify(v => v.LogError(exception, exception.Message), Times.Once);
        }
    }
}
