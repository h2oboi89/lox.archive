using Lox;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests.Lox
{
    /// <summary>
    /// Utility methods for testing <see cref="Interpreter"/>.
    /// </summary>
    class InterpreterTester
    {
        private static readonly string TEST_FILE_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
        private readonly List<string> Results = new List<string>();
        private readonly List<string> Errors = new List<string>();

        public bool ShowOptional = false;

        private void OnOut(object sender, InterpreterEventArgs e)
        {
            if (e.Optional && !ShowOptional) return;

            Results.Add(e.Message);
        }

        private void OnError(object sender, InterpreterEventArgs e)
        {
            Errors.Add(e.Message);
        }

        private readonly Queue<string> statements = new Queue<string>();
        private readonly List<string> expected = new List<string>();

        public InterpreterTester()
        {
            Interpreter.Out += OnOut;
            Interpreter.Error += OnError;
        }

        ~InterpreterTester()
        {
            Interpreter.Out -= OnOut;
            Interpreter.Error -= OnError;
        }

        /// <summary>
        /// Resets output and Interpreter.
        /// Should be called before every test (in Setup method).
        /// </summary>
        public void Reset()
        {
            Results.Clear();
            Errors.Clear();
            statements.Clear();
            expected.Clear();
            Interpreter.Reset();
        }

        /// <summary>
        /// Queues up a statement to be run during test.
        /// There can be as many of these as needed to get <see cref="Interpreter"/> in required state for <see cref="Execute(string)"/>.
        /// </summary>
        /// <param name="statement">Statement to execute.</param>
        /// <param name="expectedOutput">(optional) Expected output from statement.</param>
        public void Enqueue(string statement, params string[] expectedOutput)
        {
            statements.Enqueue(statement);

            foreach (var e in expectedOutput)
            {
                expected.Add(e);
            }
        }

        /// <summary>
        /// Queues up statements from a file to be run during test.
        /// There can be as many of these as needed to get <see cref="Interpreter"/> in required state for <see cref="Execute(string)"/>.
        /// </summary>
        /// <param name="filename">File to read lox source from.</param>
        /// <param name="expectedOutput">(optional) Expected output from file statements.</param>
        public void EnqueueFile(string filename, params string[] expectedOutput)
        {
            var file = Path.Combine(TEST_FILE_DIRECTORY, filename);
            var source = File.ReadAllText(file);

            statements.Enqueue(source);

            foreach (var e in expectedOutput)
            {
                expected.Add(e);
            }
        }

        private void ExecuteStatements(string statement = null)
        {
            if (statement != null)
            {
                statements.Enqueue(statement);
            }

            var code = string.Join("\n", statements);

            Interpreter.Run(code);
        }

        /// <summary>
        /// Executes all enqueued statements.
        /// Validates expected output from <see cref="Enqueue(string, string)"/> and <see cref="EnqueueFile(string)"/> calls.
        /// If <paramref name="expectedError"/> is not null then an expected error will be checked for.
        /// There should only be one of these per test.
        /// </summary>
        /// <param name="expectedError">Expected error</param>
        public void Execute(params string[] expectedErrors)
        {
            ExecuteStatements();

            Assert.That(Results.Count, Is.EqualTo(expected.Count));

            for (var i = 0; i < expected.Count; i++)
            {
                Assert.That(Results[i], Is.EqualTo(expected[i]));
            }

            if (expectedErrors.Length == 0)
            {
                Assert.That(Errors, Is.Empty);
            }
            else
            {
                Assert.That(Errors.Count, Is.EqualTo(expectedErrors.Length));

                foreach (var error in Errors.Enumerate())
                {
                    Assert.That(error.Value, Does.EndWith(expectedErrors[error.Index]));
                }
            }
        }
    }
}
