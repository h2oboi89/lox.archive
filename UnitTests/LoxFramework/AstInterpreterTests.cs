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

        private readonly List<string> output = new List<string>();

        [SetUp]
        public void SetUp()
        {
            interpreter = new AstInterpreter();
            interpreter.Out += OnOutput;
            output.Clear();
        }

        private void OnOutput(object sender, InterpreterEventArgs e)
        {
            output.Add(e.Message);
        }

        private IEnumerable<Statement> ScanAndParse(string input)
        {
            var tokens = Scanner.Scan(input);
            var parser = new Parser(tokens);
            return parser.Parse();
        }

        private void TestForRunTimeError(string[] invalidExpressions, string expectedErrorMessage)
        {
            foreach (var expression in invalidExpressions.Select(e => ScanAndParse(e)))
            {
                Assert.That(() => interpreter.Interpret(expression), Throws.TypeOf<LoxRunTimeException>()
                    .With.Message.Contains(expectedErrorMessage));
            }
        }

        private void TestForSuccess(Dictionary<string, string> expressionAndResults)
        {
            var i = 0;
            foreach (var entry in expressionAndResults)
            {
                interpreter.Interpret(ScanAndParse(entry.Key));

                Assert.That(output.Count, Is.EqualTo(i + 1));
                Assert.That(output[i], Is.EqualTo(entry.Value));
                i++;
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 > 2", "false" },
                { "print 2 > 1", "true" },
                { "print 2 > 2", "false" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 >= 2", "false" },
                { "print 2 >= 1", "true" },
                { "print 2 >= 2", "true" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 < 2", "true" },
                { "print 2 < 1", "false" },
                { "print 2 < 2", "false" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 <= 2", "true" },
                { "print 2 <= 1", "false" },
                { "print 2 <= 2", "true" }
            });
        }

        // ==
        [Test]
        public void VisitBinaryExpression_EqualEqual_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, string>
            {
                { "print nil == nil", "true" },
                { "print nil == 0", "false" },
                { "print nil == \"a\"", "false" },
                { "print 0 == 0", "true" },
                { "print 0 == 1", "false" },
                { "print \"a\" == \"a\"", "true" },
                { "print \"a\" == \"b\"", "false" }
            });
        }

        // !=
        [Test]
        public void VisitBinaryExpression_BangEqual_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, string>
            {
                { "print nil != nil", "false" },
                { "print nil != 0", "true" },
                { "print nil != \"a\"", "true" },
                { "print 0 != 0", "false" },
                { "print 0 != 1", "true" },
                { "print \"a\" != \"a\"", "false" },
                { "print \"a\" != \"b\"", "true" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 - 2", "-1" },
                { "print 2 - 1", "1" },
                { "print 2 - 2", "0" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 + 2", "3" },
                { "print 2 + 1", "3" },
                { "print 2 + 2", "4" },
                { "print \"1\" + \"2\"", "12" },
                { "print \"2\" + \"1\"", "21" },
                { "print \"2\" + \"2\"", "22" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 / 2", "0.5" },
                { "print 2 / 1", "2" },
                { "print 2 / 2", "1" },
                { "print 1 / 0", double.PositiveInfinity.ToString() }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 * 2", "2" },
                { "print 2 * 1", "2" },
                { "print 2 * 2", "4" }
            });
        }
        #endregion

        #region Grouping
        [Test]
        public void VisitGroupingExpression_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1 + 2 * 3", "7" },
                { "print (1 + 2) * 3", "9" }
            });
        }
        #endregion

        #region Literal
        [Test]
        public void VisitLiteralExpression_ReturnsValue()
        {
            TestForSuccess(new Dictionary<string, string>
            {
                { "print 1", "1" },
                { "print \"a\"", "a" }
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
            TestForSuccess(new Dictionary<string, string>
            {
                { "print -1", "-1.0d" },
                { "print !true", "false" },
                { "print !false", "true" }
            });
        }
        #endregion
    }
}
