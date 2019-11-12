using LoxFramework;
using LoxFramework.AST;
using LoxFramework.Parsing;
using LoxFramework.Scanning;
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class AstInterpreterTests
    {
        private AstInterpreter interpreter;

        [SetUp]
        public void SetUp()
        {
            interpreter = new AstInterpreter();
        }

        private Expression ScanAndParse(string input)
        {
            var tokens = Scanner.Scan(input);
            var parser = new Parser(tokens);
            return parser.Parse();
        }

        [Test]
        public void VisitBinaryExpression_GreaterInvalidLeftOperand_ThrowsRunTimeError()
        {
            var expression = ScanAndParse("true < 1");

            Assert.That(() => interpreter.Evaluate(expression), Throws.TypeOf<RunTimeError>()
                .With.Message.Contains("Operands must be numbers"));
        }

        [Test]
        public void VisitBinaryExpression_GreaterInvalidRightOperand_ThrowsRunTimeError()
        {
            var expression = ScanAndParse("1 < true");

            Assert.That(() => interpreter.Evaluate(expression), Throws.TypeOf<RunTimeError>()
                .With.Message.Contains("Operands must be numbers"));
        }

        [Test]
        public void VisitBinaryExpression_GreaterValid_ReturnsValue()
        {
            var expression = ScanAndParse("1 < 2");

            Assert.That(interpreter.Evaluate(expression), Is.True);
        }
    }
}
