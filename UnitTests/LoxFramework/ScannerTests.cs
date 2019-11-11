using LoxFramework;
using LoxFramework.Scanning;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class ScannerTests
    {
        private string LastError;

        [SetUp]
        public void SetUp()
        {
            Interpreter.Error += OnError;
            LastError = null;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Interpreter.Error += OnError;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Interpreter.Error -= OnError;
        }

        private void OnError(object sender, InterpreterEventArgs e)
        {
            LastError = e.Message;
        }

        private static TokenType ScanToken(string s)
        {
            return ScanTokens(s).First();
        }

        private static IEnumerable<TokenType> ScanTokens(string s)
        {
            return Scanner.Scan(s).Select(t => t.Type);
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
        public void ScanSingleCharacterTokens()
        {
            Assert.That(ScanToken("("), Is.EqualTo(TokenType.LEFT_PAREN));
            Assert.That(ScanToken(")"), Is.EqualTo(TokenType.RIGHT_PAREN));
            Assert.That(ScanToken("{"), Is.EqualTo(TokenType.LEFT_BRACE));
            Assert.That(ScanToken("}"), Is.EqualTo(TokenType.RIGHT_BRACE));
            Assert.That(ScanToken(","), Is.EqualTo(TokenType.COMMA));
            Assert.That(ScanToken("."), Is.EqualTo(TokenType.DOT));
            Assert.That(ScanToken("-"), Is.EqualTo(TokenType.MINUS));
            Assert.That(ScanToken("+"), Is.EqualTo(TokenType.PLUS));
            Assert.That(ScanToken(";"), Is.EqualTo(TokenType.SEMICOLON));
            Assert.That(ScanToken("*"), Is.EqualTo(TokenType.STAR));
        }

        [Test]
        public void ScanOneOrTwoCharacterTokens()
        {
            Assert.That(ScanToken("!"), Is.EqualTo(TokenType.BANG));
            Assert.That(ScanToken("!="), Is.EqualTo(TokenType.BANG_EQUAL));

            Assert.That(ScanToken("="), Is.EqualTo(TokenType.EQUAL));
            Assert.That(ScanToken("=="), Is.EqualTo(TokenType.EQUAL_EQUAL));

            Assert.That(ScanToken("<"), Is.EqualTo(TokenType.LESS));
            Assert.That(ScanToken("<="), Is.EqualTo(TokenType.LESS_EQUAL));

            Assert.That(ScanToken(">"), Is.EqualTo(TokenType.GREATER));
            Assert.That(ScanToken(">="), Is.EqualTo(TokenType.GREATER_EQUAL));
        }

        [Test]
        public void ScanComments()
        {
            Assert.That(ScanToken("/"), Is.EqualTo(TokenType.SLASH));
            Assert.That(ScanTokens("/ foo"), Is.EquivalentTo(new TokenType[] { TokenType.SLASH, TokenType.IDENTIFIER, TokenType.EOF }));
            Assert.That(ScanTokens("foo // bar"), Is.EquivalentTo(new TokenType[] { TokenType.IDENTIFIER, TokenType.EOF }));
            Assert.That(ScanTokens("foo // bar \n baz"), Is.EquivalentTo(new TokenType[] { TokenType.IDENTIFIER, TokenType.IDENTIFIER, TokenType.EOF }));
        }

        [Test]
        public void IgnoreWhiteSpace()
        {
            Assert.That(ScanToken(" \r\t\n"), Is.EqualTo(TokenType.EOF));
        }

        [Test]
        public void ScanStrings()
        {
            Assert.That(ScanTokens("\"foo bar\" baz"), Is.EquivalentTo(new TokenType[] { TokenType.STRING, TokenType.IDENTIFIER, TokenType.EOF }));
            Assert.That(ScanTokens("\"foo \n bar\" baz"), Is.EquivalentTo(new TokenType[] { TokenType.STRING, TokenType.IDENTIFIER, TokenType.EOF }));
        }

        [Test]
        public void ScanStrings_OpenEndedString_SetsLastError()
        {
            Assert.That(ScanToken("\" oh noes!"), Is.EqualTo(TokenType.EOF));
            Assert.That(LastError, Does.Contain("Unterminated string."));
        }

        [Test]
        public void ScanNumbers()
        {
            for (var i = 0; i < 10; i++)
            {
                Assert.That(ScanToken($"{i}"), Is.EqualTo(TokenType.NUMBER));
            }

            Assert.That(ScanToken("13"), Is.EqualTo(TokenType.NUMBER));
            Assert.That(ScanToken("9001"), Is.EqualTo(TokenType.NUMBER));

            Assert.That(ScanTokens("1."), Is.EqualTo(new TokenType[] { TokenType.NUMBER, TokenType.DOT, TokenType.EOF }));
            Assert.That(ScanToken("3.14"), Is.EqualTo(TokenType.NUMBER));
        }

        [Test]
        public void ScanIdentifiers()
        {
            Assert.That(ScanToken("x"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("y"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("i"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("foo"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("bar"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("baz"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("foo_bar23"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("_private"), Is.EqualTo(TokenType.IDENTIFIER));

            // ensure identifiers not scanned as keywords
            Assert.That(ScanToken("variation"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("varThen"), Is.EqualTo(TokenType.IDENTIFIER));
            Assert.That(ScanToken("ifelse"), Is.EqualTo(TokenType.IDENTIFIER));
        }

        [Test]
        public void RejectInvalidCharacters()
        {
            var invalidCharacters = new char[] { '\\', '@', '#', '$', '%', '^', '&' };

            foreach (var invalidCharacter in invalidCharacters)
            {
                LastError = null;

                Assert.That(ScanToken($"{invalidCharacter}"), Is.EqualTo(TokenType.EOF));
                Assert.That(LastError, Does.Contain("Unexpected character."));
            }
        }
    }
}
