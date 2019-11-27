using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void ClassPrint()
        {
            TestFile("ClassPrint.lox", new string[] { "DevonshireCream" });
        }
    }
}
