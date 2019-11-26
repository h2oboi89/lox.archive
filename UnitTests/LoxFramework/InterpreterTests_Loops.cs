using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void WhileLoop()
        {
            TestException("while;", "Expect '(' after 'while'.");
            TestException("while ( true;", "Expect ')' after condition.");

            TestStatement("var i = 0; while(i < 3) { print i; i = i + 1; }", "0", "1", "2");
        }

        [Test]
        public void ForLoop()
        {
            TestException("for;", "Expect '(' after 'for'.");
            TestException("for(var i = 0; i < 3)", "Expect ';' after loop condition.");
            TestException("for(; ; i = i + 1;", "Expect ')' after for clauses.");

            TestStatement("for(var i = 0; i < 3; i = i + 1) print i;", "0", "1", "2");

            TestStatement("var i; for(i = 0; i < 3; i = i + 1) print i;", "0", "1", "2");

            TestStatement("for(var i = 0; i < 3;) { print i; i = i + 1; }", "0", "1", "2");

            TestStatement("var i = 0; for(; i < 3;) { print i; i = i + 1; }", "0", "1", "2");
        }

        [Test]
        public void Break()
        {
            TestStatement("while(true) { print 1; break; }", "1");

            TestStatement("for(;;) { print 1; break; }", "1");

            TestStatement("for(var i = 0; i < 3; i = i + 1) { print i; for(;;) { break; } }", "0", "1", "2");

            TestException("break;", "No enclosing loop out of which to break.");

            TestException("while(true) break", "Expect ';' after 'break'.");
        }

        [Test]
        public void Continue()
        {
            TestStatement("for(var i = 0; i < 3; i = i + 1) { if (i == 2) continue; print i; }", "0", "1");

            TestStatement("var i = 0; while(i < 3) { i = i + 1; if (i == 2) continue; print i; }", "1", "3");

            TestException("continue;", "No enclosing loop out of which to continue.");

            TestException("while(true) continue", "Expect ';' after 'continue'.");
        }

        [Test]
        public void FibonnaciLoop()
        {
            var expected = new string[] {
                "0", "1", "1", "2", "3", "5", "8", "13", "21", "34", "55",
                "89", "144", "233", "377", "610", "987", "1597", "2584", "4181", "6765"
            };

            TestFile("FibonnaciLoop.lox", expected);
        }
    }
}
