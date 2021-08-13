using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlShortener.UnitTests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        private readonly CodeGenerator _codeGenerator = new CodeGenerator();

        [TestMethod]
        public void WhenGenerate_ThenCodeGenerated()
        {
            // arrange
            const int expectedLength = 8;

            // act
            var code = _codeGenerator.Generate();

            // assert
            Assert.IsTrue(code.Length == expectedLength, $"Expected code length {expectedLength} did not equal actual code length {code.Length}");
        }

        [TestMethod]
        public void GivenACode_WhenGetShortCode_ThenShortCodeReturned()
        {
            // arrange
            const string code = "di83xj23";
            const string expectedShortCode = "di";

            // act
            var shortCode = _codeGenerator.GetShortCode(code);

            // assert
            Assert.AreEqual(expectedShortCode, shortCode, $"Expected short code \"{expectedShortCode}\" did not equal actual short code {shortCode}");
        }
    }
}
