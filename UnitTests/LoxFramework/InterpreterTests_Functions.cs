
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

        [Test]
        public void FibonacciFunction()
        {
            var expected = new string[]
            {
                "0",
                "1",
                "1",
                "2",
                "3",
                "5",
                "8",
                "13",
                "21",
                "34",
                "55",
                "89",
                "144",
                "233",
                "377",
                "610",
                "987",
                "1597",
                "2584",
                "4181"
            };

            TestFile("FibonacciFunction.lox", expected);
        }

        [Test]
        public void CountFunction()
        {
            TestFile("CountFunction.lox", new string[] { "1", "2" });
        }

        [Test]
        public void MakeCounterFunction()
        {
            TestFile("MakeCounter.lox", new string[] { "1", "2" });
        }
    }
}
