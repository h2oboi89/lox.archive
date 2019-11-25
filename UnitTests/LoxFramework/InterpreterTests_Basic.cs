using NUnit.Framework;

namespace UnitTests.LoxFramework
{
    [TestFixture]
    public partial class InterpreterTests
    {
        [Test]
        public void Comments_AreIgnored()
        {
            TestStatement("// this is a comment");
        }

        [Test]
        public void Comments_GoToEndOfLine()
        {
            showOptional = true;

            TestStatement("// this is a comment \n 1 + 2;", "3");
        }

        [Test]
        public void LogicValidation()
        {
            showOptional = true;

            TestStatement("1 != 2;", "true");
            TestStatement("1 != 1;", "false");

            TestStatement("1 == 2;", "false");
            TestStatement("2 == 2;", "true");

            TestStatement("1 < 2;", "true");
            TestStatement("2 < 1;", "false");
            TestStatement("2 < 2;", "false");

            TestStatement("1 <= 2;", "true");
            TestStatement("2 <= 1;", "false");
            TestStatement("2 <= 2;", "true");

            TestStatement("1 > 2;", "false");
            TestStatement("2 > 1;", "true");
            TestStatement("2 > 2;", "false");

            TestStatement("1 >= 2;", "false");
            TestStatement("2 >= 1;", "true");
            TestStatement("2 >= 2;", "true");
        }

        [Test]
        public void Truthiness()
        {
            showOptional = true;

            TestStatement("!nil;", "true");
            TestStatement("!!nil;", "false");

            TestStatement("false;", "false");
            TestStatement("true;", "true");

            TestStatement("!1;", "false");
            TestStatement("!!1;", "true");
            TestStatement(@"!""a"";", "false");
            TestStatement(@"!!""a"";", "true");
        }

        [Test]
        public void Equality()
        {
            showOptional = true;

            TestStatement("nil == nil;", "true");

            TestStatement("nil == 0;", "false");
            TestStatement(@"nil == ""a"";", "false");

            TestStatement("false == false;", "true");
            TestStatement("false == true;", "false");
            TestStatement("true == false;", "false");
            TestStatement("true == true;", "true");

            TestStatement("false != false;", "false");
            TestStatement("false != true;", "true");
            TestStatement("true != false;", "true");
            TestStatement("true != true;", "false");

            TestStatement("0 == 0;", "true");
            TestStatement(@"""a"" == ""b"";", "false");
        }

        [Test]
        public void Unary()
        {
            showOptional = true;

            TestStatement("-1;", "-1");
            TestStatement("-(-1);", "1");
            TestStatement("!true;", "false");
            TestStatement("!false;", "true");
        }

        [Test]
        public void ArithmeticValidation()
        {
            showOptional = true;

            TestStatement("1 + 2;", "3");
            TestStatement("1 + -2;", "-1");
            TestStatement("1 - 2;", "-1");
            TestStatement("1 - -2;", "3");
            TestStatement("1 * 2;", "2");
            TestStatement("-3 * 2;", "-6");
            TestStatement("1 / 2;", "0.5");
            TestStatement("1 / 0;", double.PositiveInfinity.ToString());
        }

        [Test]
        public void DecimalNumbers()
        {
            showOptional = true;

            TestStatement("1.5 + 2.5;", "4");
            TestStatement("1 + 2.5;", "3.5");
            TestStatement("1.5 + 2;", "3.5");
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
            showOptional = true;

            TestStatement("1 + 2 * 3;", "7");
            TestStatement("(1 + 2) * 3;", "9");
        }

        [Test]
        public void MultilineString()
        {
            TestStatement("var a =\"a\nb\"; print a;", "a\nb");
        }

        [Test]
        public void StringConcatenation()
        {
            showOptional = true;

            TestStatement(@"""1"" + ""1"";", "11");
        }

        [Test]
        public void IfStatement()
        {
            TestException("if;", "Expect '(' after 'if'.");
            TestException("if ( true;", "Expect ')' after condition.");

            TestStatement("if (true) print true;", "true");
            TestStatement("if (false) print true;");
            TestStatement("if (true) print true; else print false;", "true");
            TestStatement("if (false) print true; else print false;", "false");
        }

        [Test]
        public void LogicalStatements()
        {
            TestStatement("print \"hi\" or 2;", "hi");
            TestStatement("print nil or \"yes\";", "yes");

            TestStatement("if (false and false) print true;");
            TestStatement("if (false and true) print true;");
            TestStatement("if (true and false) print true;");
            TestStatement("if (true and true) print true;", "true");

            TestStatement("if (true and true and false) print true;");
            TestStatement("if (true and true and true) print true;", "true");

            TestStatement("if (false or false) print true;");
            TestStatement("if (false or true) print true;", "true");
            TestStatement("if (true or false) print true;", "true");
            TestStatement("if (true or true) print true;", "true");

            TestStatement("if (false or false or false) print true;");
            TestStatement("if (false or false or true) print true;", "true");
        }
    }
}
