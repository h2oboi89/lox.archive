using LoxFramework;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.LoxFramework.InterpreterTests
{
    [TestFixture]
    public class Globals
    {
        private readonly List<string> Results = new List<string>();
        private readonly List<string> Errors = new List<string>();

        private void OnOut(object sender, InterpreterEventArgs e)
        {
            if (e.Optional) return;

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

        [SetUp]
        public void Setup()
        {
            Results.Clear();
            Errors.Clear();
            Interpreter.Reset();
        }

        private void TestStatement(string statement, params string[] expected)
        {
            Interpreter.Run(statement);

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(expected.Length));
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.That(Results[i], Is.EqualTo(expected[i]));
            }
        }

        [Test]
        public void Clock_Print_DisplaysNativeFunction()
        {
            TestStatement("print(clock);", "<function native>");
        }

        [Test]
        public void Reset_Print_DisplaysNativeFunction()
        {
            TestStatement("print(reset);", "<function native>");
        }

        [Test]
        public void Print_Print_DisplaysNativeFunction()
        {
            TestStatement("print(print);", "<function native>");
        }

        [Test]
        public void Clock_GetsCurrentTime()
        {
            Interpreter.Run("print(clock());");

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Does.Match(@"-?\d+(?:\.\d+)?"));
        }

        [Test]
        public void Reset_ResetsEnvironment()
        {
            Interpreter.Run("var a = 1;");
            Interpreter.Run("print(a);");

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Is.EqualTo("1"));

            Assert.That(Errors, Is.Empty);

            Interpreter.Run("reset();");
            Interpreter.Run("print(a);");

            Assert.That(Results.Count, Is.EqualTo(1));

            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.EndWith("Undefined variable 'a'."));
        }

        [Test]
        public void Print_PrintsValues()
        {
            TestStatement("print(1);", "1");
        }
    }
}