using LoxFramework;
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void GlobalToString()
        {
            TestStatement("print(clock);", "<function native>");
            TestStatement("print(reset);", "<function native>");
            TestStatement("print(print);", "<function native>");
        }

        [Test]
        public void GlobalClock()
        {
            Interpreter.Run("print(clock());");

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Does.Match(@"-?\d+(?:\.\d+)?"));
        }

        [Test]
        public void GlobalReset()
        {
            Interpreter.Run("var a = 1;");
            Interpreter.Run("print(a);");

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Is.EqualTo("1"));

            Assert.That(Errors, Is.Empty);

            Interpreter.Run("reset();");
            Interpreter.Run("print(a);");

            Assert.That(Results.Count, Is.EqualTo(1));

            Assert.That(Errors.Count, Is.EqualTo(1));
            Assert.That(Errors[0], Does.EndWith("Undefined variable 'a'."));
        }

        [Test]
        public void GlobalPrint()
        {
            TestStatement("print(1);", "1");
            TestStatement("print(1 + 2);", "3");
        }
    }
}
