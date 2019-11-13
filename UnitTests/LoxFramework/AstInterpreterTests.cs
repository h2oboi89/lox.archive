using LoxFramework;
using LoxFramework.AST;
using LoxFramework.Parsing;
using LoxFramework.Scanning;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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

        private void TestForRunTimeError(string[] invalidExpressions, string expectedErrorMessage)
        {
            foreach (var expression in invalidExpressions.Select(e => ScanAndParse(e)))
            {
                Assert.That(() => interpreter.Evaluate(expression), Throws.TypeOf<RunTimeError>()
                    .With.Message.Contains(expectedErrorMessage));
            }
        }

        private void TestForSuccess(Dictionary<string, object> expressionAndResults)
        {
            foreach (var entry in expressionAndResults)
            {
                Assert.That(interpreter.Evaluate(ScanAndParse(entry.Key)), Is.EqualTo(entry.Value));
            }
        }

        #region BinaryExpression
        // >
        [Test]
        public void VisitBinaryExpression_GreaterInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true > 1",
                "1 > true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_GreaterValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 > 2", false },
                { "2 > 1", true },
                { "2 > 2", false }
            });
        }

        // >=
        [Test]
        public void VisitBinaryExpression_GreaterEqualInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true >= 1",
                "1 >= true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_GreaterEqualValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 >= 2", false },
                { "2 >= 1", true },
                { "2 >= 2", true }
            });
        }

        // <
        [Test]
        public void VisitBinaryExpression_LessInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true < 1",
                "1 < true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_LessValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 < 2", true },
                { "2 < 1", false },
                { "2 < 2", false }
            });
        }

        // <=
        [Test]
        public void VisitBinaryExpression_LessEqualInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true <= 1",
                "1 <= true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_LessEqualValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 <= 2", true },
                { "2 <= 1", false },
                { "2 <= 2", true }
            });
        }

        // ==
        [Test]
        public void VisitBinaryExpression_EqualEqual_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "nil == nil", true },
                { "nil == 0", false },
                { "nil == \"a\"", false },
                { "0 == 0", true },
                { "0 == 1", false },
                { "\"a\" == \"a\"", true },
                { "\"a\" == \"b\"", false }
            });
        }

        // !=
        [Test]
        public void VisitBinaryExpression_BangEqual_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "nil != nil", false },
                { "nil != 0", true },
                { "nil != \"a\"", true },
                { "0 != 0", false },
                { "0 != 1", true },
                { "\"a\" != \"a\"", false },
                { "\"a\" != \"b\"", true }
            });
        }

        // -
        [Test]
        public void VisitBinaryExpression_MinusInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true - 1",
                "1 - true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_MinusValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 - 2", -1 },
                { "2 - 1", 1 },
                { "2 - 2", 0 }
            });
        }

        // +
        [Test]
        public void VisitBinaryExpression_PlusInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true + 1",
                "1 + true",
                "true + \"1\"",
                "\"1\" + true",
                "1 + \"1\"",
                "\"1\" + 1"
            }, "Operands must be two numbers or two strings");
        }

        [Test]
        public void VisitBinaryExpression_PlusValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 + 2", 3 },
                { "2 + 1", 3 },
                { "2 + 2", 4 },
                { "\"1\" + \"2\"", "12" },
                { "\"2\" + \"1\"", "21" },
                { "\"2\" + \"2\"", "22" }
            });
        }

        // /
        [Test]
        public void VisitBinaryExpression_SlashInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true / 1",
                "1 / true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_SlashValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 / 2", 0.5 },
                { "2 / 1", 2 },
                { "2 / 2", 1 }
            });
        }

        // *
        [Test]
        public void VisitBinaryExpression_StarInvalidOperands_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "true * 1",
                "1 * true"
            }, "Operands must be numbers");
        }

        [Test]
        public void VisitBinaryExpression_StarValid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 * 2", 2 },
                { "2 * 1", 2 },
                { "2 * 2", 4 }
            });
        }
        #endregion

        #region Grouping
        [Test]
        public void VisitGroupingExpression_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1 + 2 * 3", 7 },
                { "(1 + 2) * 3", 9 }
            });
        }
        #endregion

        #region Literal
        [Test]
        public void VisitLiteralExpression_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "1", 1 },
                { "\"a\"", "a" }
            });
        }
        #endregion

        #region Unary
        [Test]
        public void VisitUnaryExpression_InvalidInput_ThrowsRunTimeError()
        {
            TestForRunTimeError(new string[] {
                "-\"a\""
            }, "Operand must be a number");
        }

        [Test]
        public void VisitUnaryExpression_Valid_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, object>
            {
                { "-1", -1.0d },
                { "!true", false },
                { "!false", true }
            });
        }
        #endregion
    }
}
