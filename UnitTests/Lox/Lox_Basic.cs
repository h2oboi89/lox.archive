using NUnit.Framework;

namespace UnitTests.Lox
{
    [TestFixture]
    public partial class InterpreterTestClasses
    {
        [Test]
        public void ExpressionValues_CanBeOptionallyDisplayed()
        {
            showOptional = true;

            TestStatement("1 + 2;", "3");
        }

        [Test]
        public void Comments_AreIgnored()
        {
            TestStatement("// this is a comment");
        }

        [Test]
        public void Comments_GoToEndOfLine()
        {
            TestStatement("// this is a comment \n print(1 + 2);", "3");
        }

        [Test]
        public void LogicValidation()
        {
            TestStatement("print(1 != 2);", "true");
            TestStatement("print(1 != 1);", "false");

            TestStatement("print(1 == 2);", "false");
            TestStatement("print(2 == 2);", "true");

            TestStatement("print(1 < 2);", "true");
            TestStatement("print(2 < 1);", "false");
            TestStatement("print(2 < 2);", "false");

            TestStatement("print(1 <= 2);", "true");
            TestStatement("print(2 <= 1);", "false");
            TestStatement("print(2 <= 2);", "true");

            TestStatement("print(1 > 2);", "false");
            TestStatement("print(2 > 1);", "true");
            TestStatement("print(2 > 2);", "false");

            TestStatement("print(1 >= 2);", "false");
            TestStatement("print(2 >= 1);", "true");
            TestStatement("print(2 >= 2);", "true");
        }

        [Test]
        public void Truthiness()
        {
            TestStatement("print(!nil);", "true");
            TestStatement("print(!!nil);", "false");

            TestStatement("print(false);", "false");
            TestStatement("print(true);", "true");

            TestStatement("print(!1);", "false");
            TestStatement("print(!!1);", "true");
            TestStatement(@"print(!""a"");", "false");
            TestStatement(@"print(!!""a"");", "true");
        }

        [Test]
        public void Equality()
        {
            TestStatement("print(nil == nil);", "true");

            TestStatement("print(nil == 0);", "false");
            TestStatement(@"print(nil == ""a"");", "false");

            TestStatement("print(false == false);", "true");
            TestStatement("print(false == true);", "false");
            TestStatement("print(true == false);", "false");
            TestStatement("print(true == true);", "true");

            TestStatement("print(false != false);", "false");
            TestStatement("print(false != true);", "true");
            TestStatement("print(true != false);", "true");
            TestStatement("print(true != true);", "false");

            TestStatement("print(0 == 0);", "true");
            TestStatement(@"print(""a"" == ""b"");", "false");
        }

        [Test]
        public void Unary()
        {
            TestStatement("print(-1);", "-1");
            TestStatement("print(-(-1));", "1");
            TestStatement("print(!true);", "false");
            TestStatement("print(!false);", "true");
        }

        [Test]
        public void ArithmeticValidation()
        {
            TestStatement("print(1 + 2);", "3");
            TestStatement("print(1 + -2);", "-1");
            TestStatement("print(1 - 2);", "-1");
            TestStatement("print(1 - -2);", "3");
            TestStatement("print(1 * 2);", "2");
            TestStatement("print(-3 * 2);", "-6");
            TestStatement("print(1 / 2);", "0.5");
            TestStatement("print(1 / 0);", double.PositiveInfinity.ToString());
            TestStatement("print(33 % 5);", "3");
        }

        [Test]
        public void DecimalNumbers()
        {
            TestStatement("print(1.5 + 2.5);", "4");
            TestStatement("print(1 + 2.5);", "3.5");
            TestStatement("print(1.5 + 2);", "3.5");
        }

        [Test]
        public void InvalidOperatorOperands_ThrowsException()
        {
            TestException("-true;", "Operand must be a number.");
            TestException("1 < true;", "Operands must be numbers.");
            TestException("true > 1;", "Operands must be numbers.");
            TestException("1 + true;", "Operands must be two numbers or two strings.");
        }

        [Test]
        public void ParenthesesGrouping()
        {
            TestStatement("print(1 + 2 * 3);", "7");
            TestStatement("print((1 + 2) * 3);", "9");
        }

        [Test]
        public void MultilineString()
        {
            TestStatement("var a =\"a\nb\"; print(a);", "a\nb");
        }

        [Test]
        public void StringConcatenation()
        {

            TestStatement(@"print(""1"" + ""1"");", "11");
        }

        [Test]
        public void IfStatement()
        {
            TestException("if;", "Expect '(' after 'if'.");
            TestException("if ( true;", "Expect ')' after condition.");

            TestStatement("if (true) print(true);", "true");
            TestStatement("if (false) print(true);");
            TestStatement("if (true) print(true); else print(false);", "true");
            TestStatement("if (false) print(true); else print(false);", "false");
        }

        [Test]
        public void LogicalStatements()
        {
            TestStatement("print(\"hi\" or 2);", "hi");
            TestStatement("print(nil or \"yes\");", "yes");

            TestStatement("if (false and false) print(true);");
            TestStatement("if (false and true) print(true);");
            TestStatement("if (true and false) print(true);");
            TestStatement("if (true and true) print(true);", "true");

            TestStatement("if (true and true and false) print(true);");
            TestStatement("if (true and true and true) print(true);", "true");

            TestStatement("if (false or false) print(true);");
            TestStatement("if (false or true) print(true);", "true");
            TestStatement("if (true or false) print(true);", "true");
            TestStatement("if (true or true) print(true);", "true");

            TestStatement("if (false or false or false) print(true);");
            TestStatement("if (false or false or true) print(true);", "true");
        }
    }
}
