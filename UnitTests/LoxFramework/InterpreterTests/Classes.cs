using NUnit.Framework;

namespace UnitTests.LoxFramework.InterpreterTests
{
    [TestFixture]
    public class Classes
    {
        private readonly InterpreterTester tester = new InterpreterTester();

        [SetUp]
        public void SetUp()
        {
            tester.Reset();
        }

        private void GivenThatTestClassWasDeclaredAndInstantiated()
        {
            tester.Enqueue("class Test{}");
            tester.Enqueue("var test = Test();");
        }

        [Test]
        public void Print_ReturnsName()
        {
            tester.Enqueue("class Test{}");

            tester.Enqueue("print(Test);", "Test");

            tester.Execute();
        }

        [Test]
        public void Print_Instance_ReturnsName()
        {
            GivenThatTestClassWasDeclaredAndInstantiated();

            tester.Enqueue("print(test);", "Test instance");

            tester.Execute();
        }

        [Test]
        public void Fields_Undefined_ThrowsException()
        {
            GivenThatTestClassWasDeclaredAndInstantiated();

            tester.Enqueue("test.foo;");

            tester.Execute("Undefined property 'foo'.");
        }

        [Test]
        public void Fields_GetAndSet_ModifyAndReturnValues()
        {
            GivenThatTestClassWasDeclaredAndInstantiated();

            tester.Enqueue("test.foo = 1;");
            tester.Enqueue("print(test.foo);", "1");

            tester.Execute();
        }

        [Test]
        public void Fields_CanBeOverwritten()
        {
            GivenThatTestClassWasDeclaredAndInstantiated();

            tester.Enqueue("test.foo = 1;");
            tester.Enqueue("print(test.foo);", "1");

            tester.Enqueue("test.foo = 2;");
            tester.Enqueue("print(test.foo);", "2");

            tester.Execute();
        }

        [Test]
        public void Method_CanBeCalled()
        {
            tester.Enqueue("class Test{ foo() { return 1; } }");
            tester.Enqueue("print(Test().foo());", "1");

            tester.Execute();
        }

        [Test]
        public void UndefinedMethod_ThrowsException()
        {
            GivenThatTestClassWasDeclaredAndInstantiated();

            tester.Enqueue("test.foo();");

            tester.Execute("Undefined property 'foo'.");
        }

        [Test]
        public void This_IsBoundToInstance()
        {
            tester.EnqueueFile("Class_This_IsBoundToInstance.lox");

            tester.Enqueue("var test = Test();");
            tester.Enqueue("test.method();", "Test instance");

            tester.Enqueue("var callback = test.callback();");
            tester.Enqueue("callback();", "Test instance");

            tester.Execute();
        }

        [Test]
        public void Constructor_ReturnNotNull_ThrowsException()
        {
            tester.Enqueue("class Test { init() { return 0; } }");

            tester.Execute("Cannot return a value from an initializer.");
        }

        [Test]
        public void Constructor_ReturnNull_IsAllowed()
        {
            tester.EnqueueFile("Class_Constructor_ReturnNull_IsAllowed.lox");

            tester.Enqueue("var test = Test();");
            tester.Enqueue("print(test.foo());", "1");

            tester.Execute();
        }

        [Test]
        public void Constructor_InstantiatesObject()
        {
            tester.EnqueueFile("Class_Constructor_InstantiatesObject.lox");

            tester.Enqueue("var circle = Circle(4);");
            tester.Enqueue("print(circle.area());");
        }

        [Test]
        public void Inheritance_SuperWithoutSuperclass_ThrowsException()
        {
            tester.Enqueue("class Foo { foo() { super.foo(); } }");
            tester.Enqueue("Foo().foo();");

            tester.Execute("Cannot use 'super' in a class with no superclass.");
        }

        [Test]
        public void Inheritance_SelfInheritance_ThrowsException()
        {
            tester.Enqueue("class Foo < Foo {}");

            tester.Execute("A class cannot inherit from itself.");
        }

        [Test]
        public void Inheritance_InvalidSuperclass_ThrowsException()
        {
            tester.Enqueue("var bar = \"NotAClass\";");
            tester.Enqueue("class Foo < bar {}");

            tester.Execute("Superclass must be a class.");
        }

        [Test]
        public void Inheritance_UndefinedSuperMethod_ThrowsException()
        {
            tester.Enqueue("class Foo { foo() {} }");
            tester.Enqueue("class Bar < Foo { bar() { super.bar(); } }");
            tester.Enqueue("Bar().bar();");

            tester.Execute("Undefined property 'bar'.");
        }

        [Test]
        public void Inheritance_SuperclassMethod_CanBeCalled()
        {
            tester.Enqueue("class Foo { foo() { return 1; } }");
            tester.Enqueue("class Bar < Foo { bar() { return 2; } }");
            tester.Enqueue("var b = Bar();");

            tester.Enqueue("print(b.foo());", "1");
            tester.Enqueue("print(b.bar());", "2");

            tester.Execute();
        }

        [Test]
        public void Inheritance_ShadowedSuperclassMethod_CanBeCalledBySubclass()
        {
            tester.Enqueue("class Foo { foo() { return 1; } }");
            tester.Enqueue("class Bar < Foo { foo() { return super.foo() + 1; } }");
            tester.Enqueue("print(Bar().foo());", "2");

            tester.Execute();
        }

        [Test]
        public void KeyWord_SuperOutsideOfClass_ThrowsException()
        {
            tester.Enqueue("super.man;");

            tester.Execute("Cannot use 'super' outside of a class.");
        }

        [Test]
        public void KeyWord_ThisOutsideOfClass_ThrowsException()
        {
            tester.Enqueue("this.guy;");

            tester.Execute("Cannot use 'this' outside of a class.");
        }
    }
}
