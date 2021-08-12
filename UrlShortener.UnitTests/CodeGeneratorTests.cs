using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UrlShortener.UnitTests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        private readonly CodeGenerator _codeGenerator = new CodeGenerator();

        [TestMethod]
        public void GivenALength_WhenGenerate_ThenCodeOfGivenLengthGenerated()
        {
            // arrange
            const int length = 8;

            // act
            var code = _codeGenerator.Generate(8);

            // assert
            Assert.IsTrue(code.Length == length, $"Expected code length {length} did not equal actual code length {code.Length}");
        }

        [TestMethod]
        public void GivenACode_WhenGetShortCode_ThenShortCodeReturned()
        {
            // arrange
            const string code = "di83xj23";

            // act
            var shortCode = _codeGenerator.GetShortCode(code);

            // assert
            Assert.AreEqual("di", shortCode, $"Expected short code \"di\" did not equal actual short code {shortCode}");
        }
    }
}
