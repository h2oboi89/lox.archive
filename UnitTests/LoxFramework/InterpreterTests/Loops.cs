using NUnit.Framework;

namespace UnitTests.LoxFramework.InterpreterTests
{
    [TestFixture]
    public class Loops
    {
        private readonly InterpreterTester tester = new InterpreterTester();

        [SetUp]
        public void SetUp()
        {
            tester.Reset();
        }

        [Test]
        public void While_IteratesUntilConditionIsFalse()
        {
            tester.Enqueue("var i = 0;");
            tester.Enqueue("while(i < 3) { print(i); i = i + 1; }", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void For_IteratesUntilConditionIsFalse()
        {
            tester.Enqueue("for(var i = 0; i < 3; i = i + 1) { print(i); }", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void For_IntializerCanBeExpression()
        {
            tester.Enqueue("var i;");
            tester.Enqueue("for(i = 0; i < 3; i = i + 1) { print(i); }", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void For_InitializerCanBeNull()
        {
            tester.Enqueue("var i = 0;");
            tester.Enqueue("for(; i < 3; i = i + 1) { print(i); }", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void For_ConditionCanBeNull()
        {
            tester.Enqueue("for(var i = 0; ; i = i + 1) { print(i); if (i == 3) { break; } }", "0", "1", "2");
        }

        [Test]
        public void For_IncrementCanBeNull()
        {
            tester.Enqueue("for(var i = 0; i < 3;) { print(i); i = i + 1; }", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void For_AllPartsCanBeNull()
        {
            tester.Enqueue("var i = 0;");
            tester.Enqueue("for(;;) { if (i == 3) { break; } print(i); i = i + 1; }", "0", "1", "2");

            tester.Execute();
        }
    }
}
