using LoxFramework;
using NUnit.Framework;
using System.IO;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTestClasses
    {
        [Test]
        public void InvalidSyntax_ThrowsException()
        {
            TestException("@", "Unexpected character '@'.");

            TestException(@"""abc", "Unterminated string.");

            TestException("true = 3;", "Invalid assignment target.");

            TestException("(", "Expect expression.");

            TestException("( true;", "Expect ')' after expression.");

            TestException("true", "Expect ';' after expression.");

            TestException("return a", "Expect ';' after a return value.");

            TestException("{ print(1);", "Expect '}' after block.");

            TestException("true();", "Can only call functions and classes.");

            TestException("/", "Expect expression.");
        }

        [Test]
        public void Synchronize()
        {
            var file = Path.Combine(TEST_FILE_DIRECTORY, "SynchronizeTest.lox");
            var source = File.ReadAllText(file);
            var expected = new string[]
            {
                "[line 5] Error: Unexpected character '#'.",
                "[line 3] Error at 'fun': Expect ';' after variable declaration.",
                "[line 5] Error at ')': Expect expression.",
                "[line 7] Error at 'false': Expect '(' after 'if'.",
                "[line 9] Error at 'true': Expect '(' after 'while'.",
                "[line 11] Error at ')': Expect ';' after variable declaration.",
                "[line 19] Error at 'class': Expect variable name."
            };

            Interpreter.Run(source);

            Assert.That(Results, Is.Empty);

            Assert.That(Errors.Count, Is.EqualTo(expected.Length));

            foreach (var e in expected.Enumerate())
            {
                Assert.That(Errors[e.Index], Is.EqualTo(e.Value));
            }
        }
    }
}
