using NUnit.Framework;

namespace PowerShellClient.Tests
{
    [TestFixture]
    public class PowerShellParameterTests
    {
        [Test]
        public void ToString_WhenQuoteOptionsIsSetToQuote_WrapsValueInDoubleQuotes()
        {
            var param = new PowerShellParameter("test", ParameterQuoteOptions.Quote);

            Assert.AreEqual("\"\"\"test\"\"\"", param.ToString());
        }

        [Test]
        public void NamedParameter_ToString_WhenQuoteOptionsIsSetToQuote_WrapsValueInDoubleQuotes()
        {
            var param = new PowerShellNamedParameter("name", "test", ParameterQuoteOptions.Quote);

            Assert.AreEqual("-name \"\"\"test\"\"\"", param.ToString());
        }

        [Test]
        public void ToString_WhenQuoteOptionsIsSetToNoQuote_ValueIsNotQuoted()
        {
            var param = new PowerShellParameter("test", ParameterQuoteOptions.NoQuotes);

            Assert.AreEqual("test", param.ToString());
        }

        [Test]
        public void NamedParameter_ToString_WhenQuoteOptionsIsSetToNoQuote_ValueIsNotQuoted()
        {
            var param = new PowerShellNamedParameter("name", "test", ParameterQuoteOptions.NoQuotes);

            Assert.AreEqual("-name test", param.ToString());
        }
    }
}
