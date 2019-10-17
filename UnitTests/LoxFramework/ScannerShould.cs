using LoxFramework;
using NUnit.Framework;
using System.Linq;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class ScannerShould
    {
        private static TokenType ScanToken(string s)
        {
            return new Scanner(s).ScanTokens().Select(t => t.Type).First();
        }

        [Test]
        public void ScanKeywords()
        {
            Assert.That(ScanToken("and"), Is.EqualTo(TokenType.AND));
            Assert.That(ScanToken("class"), Is.EqualTo(TokenType.CLASS));
            Assert.That(ScanToken("else"), Is.EqualTo(TokenType.ELSE));
            Assert.That(ScanToken("false"), Is.EqualTo(TokenType.FALSE));
            Assert.That(ScanToken("for"), Is.EqualTo(TokenType.FOR));
            Assert.That(ScanToken("fun"), Is.EqualTo(TokenType.FUN));
            Assert.That(ScanToken("if"), Is.EqualTo(TokenType.IF));
            Assert.That(ScanToken("nil"), Is.EqualTo(TokenType.NIL));
            Assert.That(ScanToken("or"), Is.EqualTo(TokenType.OR));
            Assert.That(ScanToken("print"), Is.EqualTo(TokenType.PRINT));
            Assert.That(ScanToken("return"), Is.EqualTo(TokenType.RETURN));
            Assert.That(ScanToken("super"), Is.EqualTo(TokenType.SUPER));
            Assert.That(ScanToken("this"), Is.EqualTo(TokenType.THIS));
            Assert.That(ScanToken("true"), Is.EqualTo(TokenType.TRUE));
            Assert.That(ScanToken("var"), Is.EqualTo(TokenType.VAR));
            Assert.That(ScanToken("while"), Is.EqualTo(TokenType.WHILE));
        }

        [Test]
        public void NotConfuseIdentifiersAndKeyWords()
        {
            Assert.That(ScanToken("andThen"), Is.EqualTo(TokenType.IDENTIFIER));
        }
    }
}
