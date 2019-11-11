using LoxFramework.Scanning;
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class TokenShould
    {
        [Test]
        public void IntializeProperties()
        {
            Assert.That(
                new Token(TokenType.NUMBER, "3.14", 3.14, 0).ToString(),
                Is.EqualTo("NUMBER 3.14 3.14"));

            Assert.That(
                new Token(TokenType.IDENTIFIER, "foo", null, 1).ToString(),
                Is.EqualTo("IDENTIFIER foo "));
        }
    }
}
