
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void TopLevelReturn_ThrowsException()
        {
            TestException("return;", "Cannot return from top-level code.");
        }

        [Test]
        public void FunctionToString()
        {
            TestStatement("fun a() { return; } print(a);", "<function a>");
        }

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

        [Test]
        public void InvalidFunctionSyntax_ThrowsException()
        {
            TestException("fun () {}", "Expect function name.");
            TestException("fun a{}", "Expect '(' after function name.");
            TestException("fun a(1){}", "Expect parameter name.");
            TestException("fun a(b}", "Expect ')' after parameters.");

            TestException("fun foo(a, b, c) { return; } foo();", "Expected 3 arguments but got 0.");
        }

        [Test]
        public void ExcessiveParameters_ThrowsException()
        {
            var parameters = new List<string>();
            var arguments = new List<string>();

            for (var i = 0; i < 255; i++)
            {
                parameters.Add($"p{i}");
                arguments.Add($"{i}");
            }

            var paramStr = string.Join(",", parameters);
            var argStr = string.Join(",", arguments);

            // parameter and argument limit
            TestStatement($"fun foo({paramStr}) {{ return; }} foo({argStr});");

            // argument limit + 1
            argStr += ",255";

            TestException($"fun foo({paramStr}) {{ return; }} foo({argStr});", "Cannot have more than 255 arguments.");

            // parameter limit + 1
            paramStr += ",p255";

            TestException($"fun foo({paramStr}) {{ return; }}", "Cannot have more than 255 parameters.");
        }
    }
}
