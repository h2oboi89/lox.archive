using LoxFramework;
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void Variables()
        {
            TestException("a;", "Undefined variable 'a'.");
            TestException("a = 1;", "Undefined variable 'a'.");
            TestException("var a = 1; var a = 2;", "Variable 'a' already declared in this scope.");
            TestException("{ var a = a; }", "Cannot read local variable in its own initializer.");

            TestStatement("var a; print(a);", "nil");
            TestStatement("var a; a = 2; print(a);", "2");
            TestStatement("var a = 1; print(a);", "1");
            TestStatement("var a = 1; a = 2; print(a);", "2");

            TestStatement("var abc_ABC_123 = 3; print(abc_ABC_123);", "3");

        }

        [Test]
        public void VariablesInInteractiveCanBeRedeclared()
        {
            Interpreter.Reset(true);

            Interpreter.Run("var a = 1; var a = 2; print(a);");

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Is.EqualTo("2"));
        }

        [Test]
        public void Variables_Scope()
        {
            var expected = new string[]
            {
                "first",
                "second",
                "3",
                "4",
                "inner a",
                "outer b",
                "global c",
                "outer a",
                "outer b",
                "global c",
                "global a",
                "global b",
                "global c"
            };

            TestFile("VariableScope.lox", expected);
        }

        [Test]
        public void InvalidVariableSyntex_ThrowsException()
        {
            TestException("var 1;", "Expect variable name.");
            TestException("var a", "Expect ';' after variable declaration.");
        }
    }
}
