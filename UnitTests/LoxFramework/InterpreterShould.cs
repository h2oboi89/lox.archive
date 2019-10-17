using NUnit.Framework;
using LoxFramework;
using System.Collections.Generic;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public class InterpreterShould
    {
        List<string> _tokens;
        string _error;

        private void OnStatus(object sender, InterpreterEventArgs e)
        {
            _tokens.Add(e.Message);
        }

        private void FailOnError(object sender, InterpreterEventArgs e)
        {
            Assert.Fail();
        }

        private void OnError(object sender, InterpreterEventArgs e)
        {
            _error = e.Message;
        }

        [SetUp]
        public void Setup()
        {
            _tokens = new List<string>();
            _error = null;
        }

        [TearDown]
        public void TearDown()
        {
            Interpreter.Out -= OnStatus;
            Interpreter.Error -= FailOnError;
            Interpreter.Error -= OnError;
        }
        
        [Test]
        public void RunSource()
        {
            Interpreter.Out += OnStatus;
            Interpreter.Error += FailOnError;

            Interpreter.Run("foo bar baz 3.14");

            Assert.That(_tokens, Is.EquivalentTo(new string[]
            {
                "IDENTIFIER foo ",
                "IDENTIFIER bar ",
                "IDENTIFIER baz ",
                "NUMBER 3.14 3.14",
                "EOF  "
            }));
        }

        [Test]
        public void RaiseErrorEventWhenScannerThrowsException()
        {
            Interpreter.Error += OnError;

            Interpreter.Run("@");

            Assert.That(_error, Is.EqualTo("[line 1] Error: Unexpected character."));
        }
    }
}
