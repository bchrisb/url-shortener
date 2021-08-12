using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UrlShortener.Functions;
using UrlShortener.Models.Entities;
using UrlShortener.Repository;

namespace UrlShortener.UnitTests.Functions
{
    [TestClass]
    public class RedirectToFullUrlTests : TestBase
    {
        private readonly CodeGenerator _codeGenerator = new CodeGenerator();
        private readonly Mock<ITableStorageRepository<UrlCodeTableEntity>> _mockTableStorageRepository = new Mock<ITableStorageRepository<UrlCodeTableEntity>>();

        private RedirectToFullUrl _redirectToFullUrl;

        [TestInitialize]
        public void Initialize()
        {
            _redirectToFullUrl = new RedirectToFullUrl(_mockTableStorageRepository.Object, _codeGenerator);
        }

        [TestMethod]
        public async Task GivenACode_WhenRun_ThenRedirectedToFullUrl()
        {
            // arrange
            var code = "abcdefgh";
            var req = CreateMockRequest();

            var entity = new UrlCodeTableEntity { FullUrl = "https://www.foo.co.uk/bar-bar-bar-bar-bar-bar" };
            _mockTableStorageRepository
                .Setup(s => s.GetAsync("ab", code))
                .ReturnsAsync(entity);

            // act
            var response = await _redirectToFullUrl.Run(req, code, _mockLogger.Object);
            

            // assert
            Assert.IsInstanceOfType(response, typeof(RedirectResult));
            
            var result = response as RedirectResult;

            Assert.AreEqual(entity.FullUrl, result.Url);
        }

        [TestMethod]
        public async Task GivenACode_WhenRun_AndNoRecordFound_ThenNotFoundReturned()
        {
            // arrange
            var code = "abcdefgh";
            var req = CreateMockRequest();

            _mockTableStorageRepository
                .Setup(s => s.GetAsync("ab", code))
                .ReturnsAsync((UrlCodeTableEntity)null);

            // act
            var response = await _redirectToFullUrl.Run(req, code, _mockLogger.Object);

            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GivenACode_WhenRun_AndExceptionThrown_ThenInternalServerErrorReturned()
        {
            // arrange
            var code = "abcdefgh";
            var req = CreateMockRequest();

            var exception = new Exception("boom");
            _mockTableStorageRepository
                .Setup(s => s.GetAsync(_codeGenerator.GetShortCode(code), code))
                .Throws(exception);

            // act
            var response = await _redirectToFullUrl.Run(req, code, _mockLogger.Object);

            // assert
            Assert.IsInstanceOfType(response, typeof(InternalServerErrorResult));

            //_mockLogger
            //    .Verify(v => v.LogError(exception, It.Is<string>(s => s.Contains(exception.Message)), null), Times.Once);
        }
    }
}
