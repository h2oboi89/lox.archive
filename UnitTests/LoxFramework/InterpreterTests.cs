using LoxFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class InterpreterTests
    {
        private static readonly string TEST_FILE_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoxFramework", "TestFiles");
        private readonly List<string> Results = new List<string>();
        private readonly List<string> Errors = new List<string>();
        private bool showOptional = false;

        private void OnOut(object sender, InterpreterEventArgs e)
        {
            if (e.Optional && !showOptional) return;

            Results.Add(e.Message);
        }

        private void OnError(object sender, InterpreterEventArgs e)
        {
            Errors.Add(e.Message);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Interpreter.Out += OnOut;
            Interpreter.Error += OnError;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Interpreter.Out -= OnOut;
            Interpreter.Error -= OnError;
        }

        private void Reset()
        {
            Results.Clear();
            Errors.Clear();
            Interpreter.Reset();
        }

        [SetUp]
        public void Setup()
        {
            Reset();
            showOptional = false;
        }

        #region test helper methods
        private void TestFile(string filename, IEnumerable<string> expected)
        {
            var file = Path.Combine(TEST_FILE_DIRECTORY, filename);
            var source = File.ReadAllText(file);

            Interpreter.Run(source);

            Assert.That(Errors, Is.Empty);
            Assert.That(Results.Count, Is.EqualTo(expected.Count()));

            foreach (var e in expected.Enumerate())
            {
                Assert.That(Results[e.Index], Is.EqualTo(e.Value));
            }
        }

        private void TestStatement(string statement, params string[] expected)
        {
            Reset();

            Interpreter.Run(statement);

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(expected.Length));
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.That(Results[i], Is.EqualTo(expected[i]));
            }
        }

        private void TestException(string statement, string expected)
        {
            Reset();

            Interpreter.Run(statement);

            Assert.That(Results, Is.Empty);

            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.EndWith(expected));
        }
        #endregion

        [Test]
        public void Comments_AreIgnored()
        {
            TestStatement("// this is a comment");
        }

        [Test]
        public void Comments_GoToEndOfLine()
        {
            showOptional = true;

            TestStatement("// this is a comment \n 1 + 2;", "3");
        }

        [Test]
        public void LogicValidation()
        {
            showOptional = true;

            TestStatement("1 != 2;", "true");
            TestStatement("1 != 1;", "false");

            TestStatement("1 == 2;", "false");
            TestStatement("2 == 2;", "true");

            TestStatement("1 < 2;", "true");
            TestStatement("2 < 1;", "false");
            TestStatement("2 < 2;", "false");

            TestStatement("1 <= 2;", "true");
            TestStatement("2 <= 1;", "false");
            TestStatement("2 <= 2;", "true");

            TestStatement("1 > 2;", "false");
            TestStatement("2 > 1;", "true");
            TestStatement("2 > 2;", "false");

            TestStatement("1 >= 2;", "false");
            TestStatement("2 >= 1;", "true");
            TestStatement("2 >= 2;", "true");
        }

        [Test]
        public void Truthiness()
        {
            showOptional = true;

            TestStatement("!nil;", "true");
            TestStatement("!!nil;", "false");

            TestStatement("false;", "false");
            TestStatement("true;", "true");

            TestStatement("!1;", "false");
            TestStatement("!!1;", "true");
            TestStatement(@"!""a"";", "false");
            TestStatement(@"!!""a"";", "true");
        }

        [Test]
        public void Equality()
        {
            showOptional = true;

            TestStatement("nil == nil;", "true");

            TestStatement("nil == 0;", "false");
            TestStatement(@"nil == ""a"";", "false");

            TestStatement("false == false;", "true");
            TestStatement("false == true;", "false");
            TestStatement("true == false;", "false");
            TestStatement("true == true;", "true");

            TestStatement("false != false;", "false");
            TestStatement("false != true;", "true");
            TestStatement("true != false;", "true");
            TestStatement("true != true;", "false");

            TestStatement("0 == 0;", "true");
            TestStatement(@"""a"" == ""b"";", "false");
        }

        [Test]
        public void Unary()
        {
            showOptional = true;

            TestStatement("-1;", "-1");
            TestStatement("-(-1);", "1");
            TestStatement("!true;", "false");
            TestStatement("!false;", "true");
        }

        [Test]
        public void ArithmeticValidation()
        {
            showOptional = true;

            TestStatement("1 + 2;", "3");
            TestStatement("1 + -2;", "-1");
            TestStatement("1 - 2;", "-1");
            TestStatement("1 - -2;", "3");
            TestStatement("1 * 2;", "2");
            TestStatement("-3 * 2;", "-6");
            TestStatement("1 / 2;", "0.5");
            TestStatement("1 / 0;", double.PositiveInfinity.ToString());
        }

        [Test]
        public void DecimalNumbers()
        {
            showOptional = true;

            TestStatement("1.5 + 2.5;", "4");
        }

        [Test]
        public void InvalidOperatorOperands_ThrowsException()
        {
            TestException("-true;", "Operand must be a number.");
            TestException("1 < true;", "Operands must be numbers.");
            TestException("true > 1;", "Operands must be numbers.");
            TestException("1 + true;", "Operands must be two numbers or two strings.");
        }

        [Test]
        public void ParenthesesGrouping()
        {
            showOptional = true;

            TestStatement("1 + 2 * 3;", "7");
            TestStatement("(1 + 2) * 3;", "9");
        }

        [Test]
        public void MultilineString()
        {
            TestStatement("var a =\"a\nb\"; print a;", "a\nb");
        }

        [Test]
        public void StringConcatenation()
        {
            showOptional = true;

            TestStatement(@"""1"" + ""1"";", "11");
        }

        [Test]
        public void Variables()
        {
            showOptional = true;

            TestException("a;", "Undefined variable 'a'.");
            TestException("a = 1;", "Undefined variable 'a'.");
            TestException("var a = 1; var a = 2;", "Variable 'a' already declared in this scope.");

            TestStatement("var a; print a;", "nil");
            TestStatement("var a; a = 2;", "2");
            TestStatement("var a = 1; print a;", "1");
            TestStatement("var a = 1; a = 2;", "2");

            TestStatement("var abc_ABC_123 = 3; print abc_ABC_123;", "3");
        }

        [Test]
        public void VariablesInInteractiveMode()
        {
            Interpreter.Reset(true);

            Interpreter.Run("var a = 1; var a = 2; print a;");

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Is.EqualTo("2"));
        }

        [Test]
        public void Variables_Scope()
        {
            showOptional = true;

            var expected = new string[]
            {
                "first",
                "second",
                "3",
                "2",
                "4",
                "inner a",
                "outer b",
                "global c",
                "outer a",
                "outer b",
                "global c",
                "global a",
                "global b",
                "global c"
            };

            TestFile("VariableScope.lox", expected);
        }

        [Test]
        public void InvalidSyntax_ThrowsException()
        {
            TestException("@", "Unexpected character.");

            TestException(@"""abc", "Unterminated string.");

            TestException("true = 3;", "Invalid assignment target.");

            TestException("(", "Expect expression.");

            TestException("( true;", "Expect ')' after expression.");
        }

        [Test]
        public void IfStatement()
        {
            TestException("if;", "Expect '(' after 'if'.");
            TestException("if ( true;", "Expect ')' after condition.");

            TestStatement("if (true) print true;", "true");
            TestStatement("if (false) print true;");
            TestStatement("if (true) print true; else print false;", "true");
            TestStatement("if (false) print true; else print false;", "false");
        }

        [Test]
        public void LogicalStatements()
        {
            TestStatement("print \"hi\" or 2;", "hi");
            TestStatement("print nil or \"yes\";", "yes");

            TestStatement("if (false and false) print true;");
            TestStatement("if (false and true) print true;");
            TestStatement("if (true and false) print true;");
            TestStatement("if (true and true) print true;", "true");

            TestStatement("if (true and true and false) print true;");
            TestStatement("if (true and true and true) print true;", "true");

            TestStatement("if (false or false) print true;");
            TestStatement("if (false or true) print true;", "true");
            TestStatement("if (true or false) print true;", "true");
            TestStatement("if (true or true) print true;", "true");

            TestStatement("if (false or false or false) print true;");
            TestStatement("if (false or false or true) print true;", "true");
        }

        [Test]
        public void WhileLoop()
        {
            TestException("while;", "Expect '(' after 'while'.");
            TestException("while ( true;", "Expect ')' after condition.");

            TestStatement("var i = 0; while(i < 3) { print i; i = i + 1; }", "0", "1", "2");
        }

        [Test]
        public void ForLoop()
        {
            TestException("for;", "Expect '(' after 'for'.");
            TestException("for(var i = 0; i < 3)", "Expect ';' after loop condition.");
            TestException("for(; ; i = i + 1;", "Expect ')' after for clauses.");

            TestStatement("for(var i = 0; i < 3; i = i + 1) print i;", "0", "1", "2");

            TestStatement("var i; for(i = 0; i < 3; i = i + 1) print i;", "0", "1", "2");

            TestStatement("for(var i = 0; i < 3;) { print i; i = i + 1; }", "0", "1", "2");

            TestStatement("var i = 0; for(; i < 3;) { print i; i = i + 1; }", "0", "1", "2");
        }

        [Test]
        public void Break()
        {
            TestStatement("while(true) { print 1; break; }", "1");

            TestStatement("for(;;) { print 1; break; }", "1");

            TestStatement("for(var i = 0; i < 3; i = i + 1) { print i; for(;;) { break; } }", "0", "1", "2");

            TestException("break;", "No enclosing loop out of which to break.");
        }

        [Test]
        public void Continue()
        {
            TestStatement("for(var i = 0; i < 3; i = i + 1) { if (i == 2) continue; print i; }", "0", "1");

            TestStatement("var i = 0; while(i < 3) { i = i + 1; if (i == 2) continue; print i; }", "1", "3");

            TestException("continue;", "No enclosing loop out of which to continue.");
        }

        [Test]
        public void FibonnaciLoop()
        {
            var expected = new string[] {
                "0", "1", "1", "2", "3", "5", "8", "13", "21", "34", "55",
                "89", "144", "233", "377", "610", "987", "1597", "2584", "4181", "6765"
            };

            TestFile("FibonnaciLoop.lox", expected);
        }
    }
}
