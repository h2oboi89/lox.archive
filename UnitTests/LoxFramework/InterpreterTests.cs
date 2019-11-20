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

        private void OnOut(object sender, InterpreterEventArgs e)
        {
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
        }

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

        private void TestStatement(string statement, string expected = null)
        {
            Reset();

            Interpreter.Run(statement);

            Assert.That(Errors, Is.Empty);

            if (expected == null)
            {
                Assert.That(Results, Is.Empty);
            }
            else
            {
                Assert.That(Results.Count, Is.EqualTo(1));
                Assert.That(Results[0], Is.EqualTo(expected));
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

        [Test]
        public void Comments_AreIgnored()
        {
            TestStatement("// this is a comment");
        }

        [Test]
        public void Comments_GoToEndOfLine()
        {
            TestStatement("// this is a comment \n 1 + 2;", "3");
        }

        [Test]
        public void LogicValidation()
        {
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
            TestStatement("-1;", "-1");
            TestStatement("-(-1);", "1");
            TestStatement("!true;", "false");
            TestStatement("!false;", "true");
        }

        [Test]
        public void ArithmeticValidation()
        {
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
        public void StringConcatenation()
        {
            TestStatement(@"""1"" + ""1"";", "11");
        }

        [Test]
        public void ParenthesesGrouping()
        {
            TestStatement("1 + 2 * 3;", "7");
            TestStatement("(1 + 2) * 3;", "9");
        }

        [Test]
        public void Variables()
        {
            TestException("a;", "Undefined variable 'a'.");
            TestException("a = 1;", "Undefined variable 'a'.");
            TestException("var a = 1; var a = 2;", "Variable 'a' already declared in this scope.");

            TestStatement("var a; print a;", "nil");
            TestStatement("var a; a = 2;", "2");
            TestStatement("var a = 1; print a;", "1");
            TestStatement("var a = 1; a = 2;", "2");
        }

        [Test]
        public void Variables_Scope()
        {
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
    }
}
