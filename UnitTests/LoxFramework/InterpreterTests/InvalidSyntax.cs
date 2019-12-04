#pragma warning disable CA1034 // Nested types should not be visible
using NUnit.Framework;
using System.Linq;

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

        [TestFixture]
        public class Scanner : InvalidSyntax
        {
            [Test]
            public void UnexpectedCharacter_ThrowsException()
            {
                var invalidCharacters = new char[]
                {
                '~', '`', '#', '$', '%', '^',
                '[', ']', '|', '\\',
                ':', '\'',
                '?'
                };

                var expectedErrors = invalidCharacters.Select(c => $"Unexpected character '{c}'.").ToArray();

                tester.Enqueue(string.Join(" ", invalidCharacters));

                tester.Execute(expectedErrors);
            }

            [Test]
            public void UnterminatedString_ThrowsException()
            {
                tester.Enqueue("\"this is unterminated string, which is invalid.");

                tester.Execute("Unterminated string.");
            }
        }

        [TestFixture]
        public class Parser : InvalidSyntax
        {
            [TestFixture]
            public class ClassDeclaration : Parser
            {
                [Test]
                public void InvalidName_ThrowsException()
                {
                    tester.Enqueue("class");

                    tester.Execute("Expect class name.");
                }

                [Test]
                public void InvalidSuperclassName_ThrowsException()
                {
                    tester.Enqueue("class Foo <");

                    tester.Execute("Expect superclass name.");
                }

                [Test]
                public void MissingOpeningBrace_ThrowsException()
                {
                    tester.Enqueue("class Foo");

                    tester.Execute("Expect '{' before class body.");
                }

                [Test]
                public void MissingClosingBrace_ThrowsException()
                {
                    tester.Enqueue("class Foo {");

                    tester.Execute("Expect '}' after class body.");
                }
            }

            [TestFixture]
            public class FunctionDeclaration : Parser
            {
                [Test]
                public void InvalidName_ThrowsException()
                {
                    tester.Enqueue("fun");

                    tester.Execute("Expect function name.");
                }

                [Test]
                public void MissingOpenParenthesis_ThrowsException()
                {
                    tester.Enqueue("fun foo");

                    tester.Execute("Expect '(' after function name.");
                }

                [Test]
                public void ExcessiveParameters_ThrowsException()
                {
                    var parameters = new string[256];

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = $"a{i}";
                    }

                    tester.Enqueue($"fun foo({string.Join(",", parameters)}) {{ return; }}");

                    tester.Execute("Cannot have more than 255 parameters.");
                }

                [Test]
                public void InvalidParameterName_ThrowsException()
                {
                    tester.Enqueue("fun foo(1) { }");

                    tester.Execute("Expect parameter name.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("fun foo(a { }");

                    tester.Execute("Expect ')' after parameters.");
                }

                [Test]
                public void MissingOpenBrace_ThrowsException()
                {
                    tester.Enqueue("fun foo(a) }");

                    tester.Execute("Expect '{' before function body.");
                }
            }

            [TestFixture]
            public class MethodDeclaration : Parser
            {
                [Test]
                public void InvalidName_ThrowsException()
                {
                    tester.Enqueue("class Foo { 1 }");

                    tester.Execute("Expect method name.");
                }

                [Test]
                public void MissingOpenParenthesis_ThrowsException()
                {
                    tester.Enqueue("class Foo { bar }");

                    tester.Execute("Expect '(' after method name.");
                }

                [Test]
                public void ExcessiveParameters_ThrowsException()
                {
                    var parameters = new string[256];

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = $"a{i}";
                    }

                    tester.Enqueue($"class Foo {{ bar({string.Join(",", parameters)}) {{ return; }} }}");

                    tester.Execute("Cannot have more than 255 parameters.");
                }

                [Test]
                public void InvalidParameterName_ThrowsException()
                {
                    tester.Enqueue("class Foo { bar(1) { } }");

                    tester.Execute("Expect parameter name.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("class Foo { bar(a { } }");

                    tester.Execute("Expect ')' after parameters.");
                }

                [Test]
                public void MissingOpenBrace_ThrowsException()
                {
                    tester.Enqueue("class Foo { bar(a) } }");

                    tester.Execute("Expect '{' before method body.");
                }
            }

            [TestFixture]
            public class VariableDeclaration : Parser
            {
                [Test]
                public void InvalidVariableName_ThrowsException()
                {
                    tester.Enqueue("var");

                    tester.Execute("Expect variable name.");
                }

                [Test]
                public void MissingSemicolon_ThrowsException()
                {
                    tester.Enqueue("var a");

                    tester.Execute("Expect ';' after variable declaration.");
                }
            }

            [TestFixture]
            public class LoopKeyWords : Parser
            {
                [Test]
                public void Break_MissingSemicolon_ThrowsException()
                {
                    tester.Enqueue("break");

                    tester.Execute("Expect ';' after 'break'.");
                }

                [Test]
                public void Continue_MissingSemicolon_ThrowsException()
                {
                    tester.Enqueue("continue");

                    tester.Execute("Expect ';' after 'continue'.");
                }
            }

            [TestFixture]
            public class For : Parser
            {
                [Test]
                public void MissingOpeningParenthesis_ThrowsException()
                {
                    tester.Enqueue("for");

                    tester.Execute("Expect '(' after 'for'.");
                }

                [Test]
                public void MissingSemicolonAfterCondition_ThrowsException()
                {
                    tester.Enqueue("for(var i = 0; i < 5");

                    tester.Execute("Expect ';' after loop condition.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("for(var i = 0; i < 5; i = i + 1");

                    tester.Execute("Expect ')' after for clauses.");
                }
            }

            [TestFixture]
            public class If : Parser
            {
                [Test]
                public void MissingOpeningParenthesis_ThrowsException()
                {
                    tester.Enqueue("if");

                    tester.Execute("Expect '(' after 'if'.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("if (true");

                    tester.Execute("Expect ')' after condition.");
                }
            }

            [TestFixture]
            public class Return : Parser
            {
                [Test]
                public void MissingSemicolon_ThrowsException()
                {
                    tester.Enqueue("return 1");

                    tester.Execute("Expect ';' after a return value.");
                }
            }

            [TestFixture]
            public class While : Parser
            {
                [Test]
                public void MissingOpeningParenthesis_ThrowsException()
                {
                    tester.Enqueue("while");

                    tester.Execute("Expect '(' after 'while'.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("while (true");

                    tester.Execute("Expect ')' after condition.");
                }
            }

            [TestFixture]
            public class Block : Parser
            {
                [Test]
                public void MissingClosingBrace_ThrowsException()
                {
                    tester.Enqueue("{");

                    tester.Execute("Expect '}' after block.");
                }
            }

            [TestFixture]
            public class Expression : Parser
            {
                [Test]
                public void MissingSemicolon_ThrowsException()
                {
                    tester.Enqueue("1");

                    tester.Execute("Expect ';' after expression.");
                }
            }

            [TestFixture]
            public class Assignment : Parser
            {
                [Test]
                public void InvalidTarget_ThrowsException()
                {
                    tester.Enqueue("1 = 2;");

                    tester.Execute("Invalid assignment target.");
                }
            }

            [TestFixture]
            public class Call : Parser
            {
                [Test]
                public void InvalidTarget_ThrowsException()
                {
                    tester.Enqueue("foo.");

                    tester.Execute("Expect property name after '.'.");
                }

                [Test]
                public void ExcessiveArguments_ThrowsException()
                {
                    var arguments = new string[256];

                    for (var i = 0; i < arguments.Length; i++)
                    {
                        arguments[i] = $"{i}";
                    }

                    tester.Enqueue($"foo({string.Join(",", arguments)});");

                    tester.Execute("Cannot have more than 255 arguments.");
                }

                [Test]
                public void MissingClosingParenthesis_ThrowsException()
                {
                    tester.Enqueue("foo(1");

                    tester.Execute("Expect ')' after arguments.");
                }
            }

            [TestFixture]
            public class Primary : Parser
            {
                [Test]
                public void Class_MissingPeriodAfterSuper_ThrowsException()
                {
                    tester.Enqueue("super");

                    tester.Execute("Expect '.' after 'super'.");
                }

                [Test]
                public void Class_InvalidSuperclassMethodName_ThrowsException()
                {
                    tester.Enqueue("super.");

                    tester.Execute("Expect superclass method name.");
                }

                [Test]
                public void Expression_UnclosedParenthesis_ThrowsException()
                {
                    tester.Enqueue("( 1 + 1");

                    tester.Execute("Expect ')' after expression.");
                }

                [Test]
                public void EmptyInput_ThrowsException()
                {
                    tester.Enqueue(";");

                    tester.Execute("Expect expression.");
                }
            }

            [TestFixture]
            public class Synchronize : Parser
            {
                [Test]
                public void AfterSemicolon()
                {
                    tester.Enqueue("; ;");

                    tester.Execute("Expect expression.", "Expect expression.");
                }

                [Test]
                public void OnClass()
                {
                    tester.Enqueue("fun 1 class Foo { } ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnFunction()
                {
                    tester.Enqueue("fun 1 fun foo() { return; } ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnVariable()
                {
                    tester.Enqueue("fun 1 var a; ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnFor()
                {
                    tester.Enqueue("fun 1 for(;;) { break; } ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnIf()
                {
                    tester.Enqueue("fun 1 if (true) { return; } ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnWhile()
                {
                    tester.Enqueue("fun 1 while(true) { break; } ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }

                [Test]
                public void OnReturn()
                {
                    tester.Enqueue("fun 1 return 1; ;");

                    tester.Execute("Expect function name.", "Expect expression.");
                }
            }
        }

        [TestFixture]
        public class Resolver : InvalidSyntax
        {
            [Test]
            public void TopLevelReturn_ThrowsException()
            {
                tester.Enqueue("return;");

                tester.Execute("Cannot return from top-level code.");
            }
        }

        [TestFixture]
        public class AstInterpreter : InvalidSyntax
        {
            [Test]
            public void UnaryMinus_OperandMustBeNumber()
            {
                tester.Enqueue("-true;");

                tester.Execute("Operand must be a number.");
            }

            [Test]
            public void BinaryGreater_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true > 0;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryGreater_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 > true;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryGreaterEqual_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true >= 0;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryGreaterEqual_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 >= true;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryLess_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true < 0;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryLess_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 < true;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryLessEqual_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true <= 0;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryLessEqual_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 <= true;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryMinus_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true - 0;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryMinus_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 - true;");

                tester.Execute("Operands must be numbers.");
            }

            [Test]
            public void BinaryAdd_LeftOperandMustBeNumber()
            {
                tester.Enqueue("true + 0;");

                tester.Execute("Operands must be two numbers or two strings.");
            }

            [Test]
            public void BinaryAdd_RightOperandMustBeNumber()
            {
                tester.Enqueue("0 + true;");

                tester.Execute("Operands must be two numbers or two strings.");
            }

            [Test]
            public void BinaryConcatenation_LeftOperandMustBeString()
            {
                tester.Enqueue("true + \"a\";");

                tester.Execute("Operands must be two numbers or two strings.");
            }

            [Test]
            public void BinaryConcatenation_RightOperandMustBeString()
            {
                tester.Enqueue("\"a\" + true;");

                tester.Execute("Operands must be two numbers or two strings.");
            }

            [Test]
            public void Call_InvalidCall_ThrowsException()
            {
                tester.Enqueue("var a;");
                tester.Enqueue("a();");

                tester.Execute("Can only call functions and classes.");
            }

            [Test]
            public void Call_InvalidArity_ThrowsException()
            {
                tester.Enqueue("fun foo() { return; }");
                tester.Enqueue("foo(1);");

                tester.Execute("Expected 0 arguments but got 1.");
            }

            [Test]
            public void InvalidGet_ThrowsException()
            {
                tester.Enqueue("var a;");
                tester.Enqueue("a.foo;");

                tester.Execute("Only instances have properties.");
            }

            [Test]
            public void InvalidSet_ThrowsException()
            {
                tester.Enqueue("var a;");
                tester.Enqueue("a.foo = 1;");

                tester.Execute("Only instances have fields.");
            }
        }
    }
}

#pragma warning restore CA1034 // Nested types should not be visible