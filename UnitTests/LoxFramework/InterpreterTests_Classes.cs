using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void Class_Print_ReturnsName()
        {
            TestStatement("class Test{} print(Test);", "Test");
        }

        [Test]
        public void Class_InstantiatedPrint_ReturnsName()
        {
            TestStatement("class Test {} var test = Test(); print(test);", "Test instance");
        }
    }
}
