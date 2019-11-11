using LoxFramework;
using LoxFramework.AST;
using LoxFramework.Parsing;
using LoxFramework.Scanning;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class ParserTests
    {
        private readonly AstPrinter printer = new AstPrinter();

        private List<string> Errors;

        private void OnError(object sender, InterpreterEventArgs e)
        {
            Errors.Add(e.Message);
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

        [SetUp]
        public void SetUp()
        {
            Errors = new List<string>();
        }

        [Test]
        public void Parse_EqualityNotEqual()
        {
            var tokens = Scanner.Scan("1 != 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(!= 1 2)"));
        }

        [Test]
        public void Parse_EqualityEqual()
        {
            var tokens = Scanner.Scan("1 == 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(== 1 2)"));
        }

        [Test]
        public void Parse_ComparisonGreater()
        {
            var tokens = Scanner.Scan("1 > 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(> 1 2)"));
        }

        [Test]
        public void Parse_ComparisonGreaterEqual()
        {
            var tokens = Scanner.Scan("1 >= 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(>= 1 2)"));
        }

        [Test]
        public void Parse_ComparisonLess()
        {
            var tokens = Scanner.Scan("1 < 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(< 1 2)"));
        }

        [Test]
        public void Parse_ComparisonLessEqual()
        {
            var tokens = Scanner.Scan("1 <= 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(<= 1 2)"));
        }

        [Test]
        public void Parse_AdditionPlus()
        {
            var tokens = Scanner.Scan("1 + 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(+ 1 2)"));
        }

        [Test]
        public void Parse_AdditionMinus()
        {
            var tokens = Scanner.Scan("1 - 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(- 1 2)"));
        }

        [Test]
        public void Parse_MultiplicationStar()
        {
            var tokens = Scanner.Scan("1 * 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(* 1 2)"));
        }

        [Test]
        public void Parse_MultiplicationSlash()
        {
            var tokens = Scanner.Scan("1 / 2");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(/ 1 2)"));
        }

        [Test]
        public void Parse_UnaryBang()
        {
            var tokens = Scanner.Scan("!true");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(! True)"));
        }

        [Test]
        public void Parse_UnaryMinus()
        {
            var tokens = Scanner.Scan("-1");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(- 1)"));
        }

        [Test]
        public void Parse_PrimaryFalse()
        {
            var tokens = Scanner.Scan("false");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("False"));
        }

        [Test]
        public void Parse_PrimaryTrue()
        {
            var tokens = Scanner.Scan("true");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("True"));
        }

        [Test]
        public void Parse_PrimaryNil()
        {
            var tokens = Scanner.Scan("nil");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("nil"));
        }

        [Test]
        public void Parse_PrimaryNumber()
        {
            var tokens = Scanner.Scan("1");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("1"));
        }

        [Test]
        public void Parse_PrimaryString()
        {
            var tokens = Scanner.Scan("\"bob\"");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("bob"));
        }

        [Test]
        public void Parse_PrimaryGrouping()
        {
            var tokens = Scanner.Scan("( 1 + 2 )");

            var expression = printer.Print(new Parser(tokens).Parse());

            Assert.That(expression, Is.EqualTo("(group (+ 1 2))"));
        }

        [Test]
        public void Parse_PrimaryUnmatchedParenthesis()
        {
            var tokens = Scanner.Scan("( 1 + 2");

            var expression = new Parser(tokens).Parse();

            Assert.That(expression, Is.Null);

            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.Contain("Expect ')' after expression"));
        }
    }
}
