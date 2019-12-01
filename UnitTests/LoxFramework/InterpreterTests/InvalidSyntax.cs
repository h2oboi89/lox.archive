using NUnit.Framework;

namespace UnitTests.LoxFramework.InterpreterTests
{
    [TestFixture]
    public class InvalidSyntax
    {
        private readonly InterpreterTester tester = new InterpreterTester();

        [SetUp]
        public void SetUp()
        {
            tester.Reset();
        }

        [Test]
        public void GetOnNonInstance_ThrowsException()
        {
            tester.Enqueue("1.foo;");

            tester.Execute("Only instances have properties.");
        }

        [Test]
        public void SetOnNonInstance_ThrowsException()
        {
            tester.Enqueue("1.foo = 2;");

            tester.Execute("Only instances have fields.");
        }

        [Test]
        public void GlobalThis_ThrowsException()
        {
            tester.Enqueue("print(this);");

            tester.Execute("Cannot use 'this' outside of a class.");
        }

        [Test]
        public void BlockThis_ThrowsException()
        {
            tester.Enqueue("fun notAMethod() { print(this); }");

            tester.Execute("Cannot use 'this' outside of a class.");
        }

        [Test]
        public void GlobalSuper_ThrowsException()
        {
            tester.Enqueue("super.foo();");

            tester.Execute("Cannot use 'super' outside of a class.");
        }

        [Test]
        public void MissingSuperclassName_ThrowsException()
        {
            tester.Enqueue("class Foo < {}");

            tester.Execute("Expect superclass name.");
        }

        [Test]
        public void NonClassSuperclass_ThrowsException()
        {
            tester.Enqueue("var NotAClass = 1;");
            tester.Enqueue("class Foo < NotAClass {}");

            tester.Execute("Superclass must be a class.");
        }

        [Test]
        public void SuperWithoutMethodName_ThrowsException()
        {
            tester.Enqueue("class Foo { foo() {} }");
            tester.Enqueue("class Bar < Foo { foo() { super; } }");

            tester.Execute("Expect '.' after 'super'.");
        }

        [Test]
        public void SuperWithInvalidMethodName_ThrowsException()
        {
            tester.Enqueue("class Foo { foo() {} }");
            tester.Enqueue("class Bar < Foo { foo() { super.1(); } }");

            tester.Execute("Expect superclass method name.");
        }
    }
}
