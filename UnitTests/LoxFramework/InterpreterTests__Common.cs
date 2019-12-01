using LoxFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTestClasses
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
    }
}
