
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void HelloFunction()
        {
            TestFile("HelloWorld.lox", new string[] { "Hello, Lox World!" });
        }
    }
}
