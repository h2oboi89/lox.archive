using LoxFramework;
using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void Clock()
        {
            Interpreter.Run("print clock();");

            Assert.That(Errors, Is.Empty);

            Assert.That(Results.Count, Is.EqualTo(1));
            Assert.That(Results[0], Does.Match(@"-?\d+(?:\.\d+)?"));
        }
    }
}
