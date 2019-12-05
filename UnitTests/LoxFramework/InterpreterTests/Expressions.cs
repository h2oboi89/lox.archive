using NUnit.Framework;

namespace UnitTests.LoxFramework.InterpreterTests
{
    [TestFixture]
    public class Expressions
    {
        private readonly InterpreterTester tester = new InterpreterTester();

        [SetUp]
        public void SetUp()
        {
            tester.Reset();
            tester.ShowOptional = true;
        }

        [Test]
        public void Expression_OutputIsOptional()
        {
            tester.ShowOptional = false;

            tester.Enqueue("1 + 2;");
            tester.Enqueue("print(1 + 2);", "3");

            tester.Execute();
        }

        [Test]
        public void Comments_AreIgnored()
        {
            tester.Enqueue("// 1 + 2;");
            tester.Enqueue("1 + 2;", "3");

            tester.Execute();
        }

        [Test]
        public void Comments_NewLinesEndComments()
        {
            tester.Enqueue("// this is a comment \n 1 + 2;", "3");

            tester.Execute();
        }

        [Test]
        public void Truthiness_Nil_IsFalse()
        {
            tester.Enqueue("!!nil;", "false");

            tester.Execute();
        }

        [Test]
        public void Truthiness_Booleans_AreThemselves()
        {
            tester.Enqueue("false;", "false");
            tester.Enqueue("!false;", "true");
            tester.Enqueue("true;", "true");
            tester.Enqueue("!true;", "false");

            tester.Execute();
        }

        [Test]
        public void Truthiness_Other_AreAllTrue()
        {
            tester.Enqueue("!!0;", "true");
            tester.Enqueue("!!1;", "true");

            tester.Enqueue("!!\"a\";", "true");
            tester.Enqueue("!!\"b\";", "true");

            tester.Enqueue("fun foo() {}");
            tester.Enqueue("!!foo;", "true");

            tester.Enqueue("class Bar {}");
            tester.Enqueue("var bar = Bar();");
            tester.Enqueue("!!Bar;", "true");
            tester.Enqueue("!!bar;", "true");

            tester.Execute();
        }

        [Test]
        public void Equality_NilAndNil_IsTrue()
        {
            tester.Enqueue("nil == nil;", "true");
            tester.Enqueue("nil != nil;", "false");

            tester.Execute();
        }

        [Test]
        public void Equality_NilAndAnything_IsFalse()
        {
            // numbers
            tester.Enqueue("nil == 0;", "false");
            tester.Enqueue("nil != 0;", "true");

            // strings
            tester.Enqueue("nil == \"a\";", "false");
            tester.Enqueue("nil != \"a\";", "true");

            // booleans
            tester.Enqueue("nil == true;", "false");
            tester.Enqueue("nil == false;", "false");

            tester.Enqueue("nil != true;", "true");
            tester.Enqueue("nil != false;", "true");

            // functions
            tester.Enqueue("fun foo() {}");
            tester.Enqueue("nil == foo;", "false");
            tester.Enqueue("nil != foo;", "true");

            // classes
            tester.Enqueue("class Bar {}");
            tester.Enqueue("var bar = Bar();");
            tester.Enqueue("nil == Bar;", "false");
            tester.Enqueue("nil == bar;", "false");
            tester.Enqueue("nil != Bar;", "true");
            tester.Enqueue("nil != bar;", "true");

            tester.Execute();
        }

        [Test]
        public void Equality_ValuesAreCompared()
        {
            // numbers
            tester.Enqueue("0 == 0;", "true");
            tester.Enqueue("0 != 0;", "false");

            tester.Enqueue("0 == 1;", "false");
            tester.Enqueue("0 != 1;", "true");

            // strings
            tester.Enqueue("\"a\" == \"a\";", "true");
            tester.Enqueue("\"a\" != \"a\";", "false");

            tester.Enqueue("\"a\" == \"b\";", "false");
            tester.Enqueue("\"a\" != \"b\";", "true");

            // booleans
            tester.Enqueue("false == false;", "true");
            tester.Enqueue("false != false;", "false");

            tester.Enqueue("false == true;", "false");
            tester.Enqueue("false != true;", "true");

            tester.Enqueue("true == false;", "false");
            tester.Enqueue("true != false;", "true");

            tester.Enqueue("true == true;", "true");
            tester.Enqueue("true != true;", "false");

            // functions
            tester.Enqueue("fun foo() {} fun bar() {}");
            tester.Enqueue("foo == foo;", "true");
            tester.Enqueue("foo == bar;", "false");

            // classes
            tester.Enqueue("class Foo {} class Bar {}");
            tester.Enqueue("var f = Foo(); var b = Bar(); var c = Foo();");
            tester.Enqueue("Foo == Foo;", "true");
            tester.Enqueue("Foo == Bar;", "false");
            tester.Enqueue("f == f;", "true");
            tester.Enqueue("f == c;", "false");
            tester.Enqueue("f == b;", "false");

            tester.Execute();
        }
    }
}
