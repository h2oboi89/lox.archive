using NUnit.Framework;

namespace UnitTests.Lox
{
    [TestFixture]
    public class Functions
    {
        private readonly InterpreterTester tester = new InterpreterTester();

        [SetUp]
        public void SetUp()
        {
            tester.Reset();
        }

        [Test]
        public void TopLevelReturn_ThrowsException()
        {
            tester.Enqueue("return;");

            tester.Execute("Cannot return from top-level code.");
        }

        [Test]
        public void Call_InvalidCall_ThrowsException()
        {
            tester.Enqueue("var a;");
            tester.Enqueue("a();");

            tester.Execute("Can only call functions and classes.");
        }

        [Test]
        public void Call_InvalidArity_ThrowsException()
        {
            tester.Enqueue("fun foo() { return; }");
            tester.Enqueue("foo(1);");

            tester.Execute("Expected 0 arguments but got 1.");
        }

        [Test]
        public void FunctionToString()
        {
            tester.Enqueue("fun a() { return; } print(a);", "<function a>");

            tester.Execute();
        }

        [Test]
        public void FunctionsCanReturnValues()
        {
            tester.EnqueueFile("HelloWorld.lox", "Hello, Lox World!");

            tester.Execute();
        }

        [Test]
        public void FunctionsCanRecursivelyCallThemselves()
        {
            tester.EnqueueFile("FibonacciFunction.lox", "0", "1", "1", "2", "3", "5", "8", "13", "21", "34");

            tester.Execute();
        }

        [Test]
        public void CountFunction()
        {
            tester.EnqueueFile("CountFunction.lox", "1", "2");

            tester.Execute();
        }

        [Test]
        public void FunctionsCreateAScopeThatNestedFunctionsBindTo()
        {
            tester.EnqueueFile("MakeCounter.lox", "1", "2");

            tester.Execute();
        }
    }
}
