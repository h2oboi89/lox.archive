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
            TestStatement("class Test{} var test = Test(); print(test);", "Test instance");
        }

        [Test]
        public void Class_Fields_CanBeSetAndGet()
        {
            TestStatement("class Test{} var test = Test(); test.foo = 1; print(test.foo);", "1");
        }

        [Test]
        public void Class_Field_CanBeOverwritten()
        {
            TestStatement(
                "class Test{} var test = Test(); test.foo = 1; print(test.foo); test.foo = 2; print(test.foo);"
                , "1", "2"
                );
        }

        [Test]
        public void Class_GetOnNonInstance_ThrowsException()
        {
            TestException("1.foo;", "Only instances have properties.");
        }

        [Test]
        public void Class_SetOnNonInstance_ThrowsException()
        {
            TestException("1.foo = 2;", "Only instances have fields.");
        }

        [Test]
        public void Class_UndefinedFieldGet_ThrowsException()
        {
            TestException("class Test{} var test = Test(); test.foo;", "Undefined property 'foo'.");
        }

        [Test]
        public void Class_Method_Executes()
        {
            TestStatement("class Test{ foo() { print(1); } } Test().foo();", "1");
        }

        [Test]
        public void Class_UndefinedMethod_ThrowsException()
        {
            TestException("class Test{ foo() { print(1); } } Test().bar();", "Undefined property 'bar'.");
        }

        [Test]
        public void Class_ThisTest()
        {
            TestFile("This.lox", new string[] { "Test instance", "Test instance" });
        }

        [Test]
        public void Class_InvalidThis_ThrowsException()
        {
            TestException("print(this);", "Cannot use 'this' outside of a class.");
            TestException("fun notAMethod() { print(this); }", "Cannot use 'this' outside of a class.");
        }
    }
}
