using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void InvalidSyntax_ThrowsException()
        {
            TestException("@", "Unexpected character.");

            TestException(@"""abc", "Unterminated string.");

            TestException("true = 3;", "Invalid assignment target.");

            TestException("(", "Expect expression.");

            TestException("( true;", "Expect ')' after expression.");
        }
    }
}
