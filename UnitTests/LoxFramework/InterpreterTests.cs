using LoxFramework;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class InterpreterTests
    {
        private string Result;
        private List<string> Errors;

        private void OnStatus(object sender, InterpreterEventArgs e)
        {
            Result = e.Message;
        }

        private void OnError(object sender, InterpreterEventArgs e)
        {
            Errors.Add(e.Message);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Interpreter.Out += OnStatus;
            Interpreter.Error += OnError;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Interpreter.Out -= OnStatus;
            Interpreter.Error -= OnError;
        }

        [SetUp]
        public void Setup()
        {
            Result = null;
            Errors = new List<string>();
        }

        [Test]
        public void Run_BasicArithmetic_ReturnsValue()
        {
            Interpreter.Run("1 + 2 * 3");

            Assert.That(Result, Is.EqualTo("7"));
            Assert.That(Errors, Is.Empty);
        }

        [Test]
        public void Run_BasicArithmeticWithParens_ReturnsValue()
        {
            Interpreter.Run("(1 + 2) * 3");

            Assert.That(Result, Is.EqualTo("9"));
            Assert.That(Errors, Is.Empty);
        }

        [Test]
        public void Run_LiteralString_ReturnsValue()
        {
            Interpreter.Run("\"1\"");

            Assert.That(Result, Is.EqualTo("1"));
            Assert.That(Errors, Is.Empty);
        }

        [Test]
        public void Run_LiteralNumber_ReturnsValue()
        {
            Interpreter.Run("1");

            Assert.That(Result, Is.EqualTo("1"));
            Assert.That(Errors, Is.Empty);
        }

        [Test]
        public void Run_EmptyInput_DoesNothing()
        {
            Interpreter.Run("");

            Assert.That(Result, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void Run_InvalidSyntax_ReportsError()
        {
            Interpreter.Run("@");

            Assert.That(Result, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(2));
            Assert.That(Errors[0], Does.Contain("Unexpected character"));
            Assert.That(Errors[1], Does.Contain("Expect expression"));
        }

        [Test]
        public void Run_InvalidExpression_ReportsError()
        {
            Interpreter.Run("foo bar baz");

            Assert.That(Result, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.Contain("Expect expression"));
        }

        [Test]
        public void Run_RunTimeError_ReportsError()
        {
            Interpreter.Run("true - 1");

            Assert.That(Result, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.Contain("Operands must be numbers"));
        }
    }
}
