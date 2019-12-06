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

        [Test]
        public void While_ContinueAndBreak_ModifyIterationInLoop()
        {
            tester.Enqueue("var i = 0;");
            tester.Enqueue("while(true) { if (i == 10) { break; } if(i % 2 == 0) { print(i);} i = i + 1; }", "0", "2", "4", "6", "8");

            tester.Execute();
        }

        [Test]
        public void For_ContinueAndBreak_ModifyIterationInLoop()
        {
            tester.Enqueue("var i = 0;");
            tester.Enqueue("for(;;) { if (i == 10) { break; } if(i % 2 == 0) { print(i);} i = i + 1; }", "0", "2", "4", "6", "8");

            tester.Execute();
        }

        [Test]
        public void Break_OutsideOfLoop_ThrowsException()
        {
            tester.Enqueue("break;");

            tester.Execute("No enclosing loop out of which to break.");
        }

        [Test]
        public void Continue_OutsideOfLoop_ThrowsException()
        {
            tester.Enqueue("continue;");

            tester.Execute("No enclosing loop out of which to continue.");
        }

        [Test]
        public void Continue_Nested()
        {
            tester.EnqueueFile("Loops_Nested_Continue.lox", "0", "2", "1", "3", "2", "4");

            tester.Execute();
        }

        [Test]
        public void Break_Nested()
        {
            tester.EnqueueFile("Loops_Nested_Break.lox", "0", "1", "2");

            tester.Execute();
        }

        [Test]
        public void FibonnaciLoop()
        {
            var expected = new string[] {
                "0", "1", "1", "2", "3", "5", "8", "13", "21", "34", "55",
                "89", "144", "233", "377", "610", "987", "1597", "2584", "4181", "6765"
            };

            tester.EnqueueFile("FibonnaciLoop.lox", expected);

            tester.Execute();
        }
    }
}
