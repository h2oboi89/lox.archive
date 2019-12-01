using LoxFramework.AST;
using LoxFramework.Scanning;
using System;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class AstInterpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private Environment environment;
        private Environment globals;
        private Dictionary<Expression, int> locals;

        public AstInterpreter()
        {
            Reset();
        }

        private static Token GlobalFunctionName(string name)
        {
            return new Token(TokenType.FUN, name);
        }

        public void Reset()
        {
            globals = new Environment();
            environment = globals;

            locals = new Dictionary<Expression, int>();

            globals.Define(GlobalFunctionName("clock"), new Globals.Clock());
            globals.Define(GlobalFunctionName("print"), new Globals.Print());
            globals.Define(GlobalFunctionName("reset"), new Globals.Reset());
        }

        public void Interpret(IEnumerable<Statement> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (LoxRunTimeException e)
            {
                Interpreter.RunTimeError(e);
            }
        }

        private void Execute(Statement statement)
        {
            statement?.Accept(this);
        }

        internal void Resolve(Expression expression, int depth)
        {
            locals.Add(expression, depth);
        }

        internal void ExecuteBlock(IEnumerable<Statement> statements, Environment blockEnvironment)
        {
            var enclosingEnvironment = environment;

            try
            {
                environment = blockEnvironment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                environment = enclosingEnvironment;
            }
        }

        public event EventHandler<InterpreterEventArgs> Out;

        private static string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is bool bObj) return bObj ? "true" : "false";

            return obj.ToString();
        }

        internal void RaiseOut(object obj, bool optional = false)
        {
            Out?.Invoke(this, new InterpreterEventArgs(Stringify(obj), optional));
        }

        #region Expressions
        private object Evaluate(Expression expression)
        {
            return expression?.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool bObj)
            {
                return bObj;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;

            throw new LoxRunTimeException(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;

            throw new LoxRunTimeException(op, "Operands must be numbers.");
        }

        private object LookUpVariable(Token name, Expression expression)
        {
            if (!locals.ContainsKey(expression))
            {
                return globals.Get(name);
            }

            var distance = locals[expression];

            return environment.Get(name, distance);
        }

        public object VisitBinaryExpression(BinaryExpression expression)
        {
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

            switch (expression.Op.Type)
            {
                // Comparisons
                case TokenType.GREATER:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                // Arithmetic
                case TokenType.MINUS:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double dLeft && right is double dRight)
                    {
                        return dLeft + dRight;
                    }

                    if (left is string sLeft && right is string sRight)
                    {
                        return sLeft + sRight;
                    }

                    throw new LoxRunTimeException(expression.Op, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expression.Op, left, right);
                    return (double)left * (double)right;
            }

            // unreachable
            return null;
        }

        public object VisitCallExpression(CallExpression expression)
        {
            var callee = Evaluate(expression.Callee);

            var arguments = new List<object>();

            foreach (var argument in expression.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ILoxCallable function))
            {
                throw new LoxRunTimeException(expression.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity)
            {
                throw new LoxRunTimeException(expression.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitGetExpression(GetExpression expression)
        {
            var obj = Evaluate(expression.Obj);

            if (obj is LoxInstance loxInstance)
            {
                return loxInstance[expression.Name];
            }

            throw new LoxRunTimeException(expression.Name, "Only instances have properties.");
        }

        public object VisitGroupingExpression(GroupingExpression expression)
        {
            return Evaluate(expression.Expression);
        }

        public object VisitLiteralExpression(LiteralExpression expression)
        {
            return expression.Value;
        }

        public object VisitLogicalExpression(LogicalExpression expression)
        {
            var left = Evaluate(expression.Left);

            if (expression.Op.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expression.Right);
        }

        public object VisitSetExpression(SetExpression expression)
        {
            var obj = Evaluate(expression.Obj);

            if (obj is LoxInstance loxInstance)
            {
                var value = Evaluate(expression.Value);

                loxInstance[expression.Name] = value;

                return value;
            }
            else
            {
                throw new LoxRunTimeException(expression.Name, "Only instances have fields.");
            }
        }

        public object VisitThisExpression(ThisExpression expression)
        {
            return LookUpVariable(expression.Keyword, expression);
        }

        public object VisitUnaryExpression(UnaryExpression expression)
        {
            var right = Evaluate(expression.Right);

            switch (expression.Op.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(expression.Op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            // unreachable
            return null;
        }

        public object VisitVariableExpression(VariableExpression expression)
        {
            return LookUpVariable(expression.Name, expression);
        }

        public object VisitAssignmentExpression(AssignmentExpression expression)
        {
            var value = Evaluate(expression.Value);

            if (!locals.ContainsKey(expression))
            {
                globals.Assign(expression.Name, value);
            }
            else
            {
                var distance = locals[expression];

                environment.Assign(expression.Name, value, distance);
            }

            return value;
        }
        #endregion

        #region Statements
        public object VisitBlockStatement(BlockStatement statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment));

            return null;
        }

        public object VisitBreakStatement(BreakStatement statement)
        {
            throw new LoxBreakException();
        }

        public object VisitClassStatement(ClassStatement statement)
        {
            object superclass = null;

            if (statement.Superclass != null)
            {
                superclass = Evaluate(statement.Superclass);

                if (!(superclass is LoxClass))
                {
                    throw new LoxRunTimeException(statement.Superclass.Name, "Superclass must be a class.");
                }
            }

            environment.Define(statement.Name, null);

            var methods = new Dictionary<string, LoxFunction>();
            foreach (var method in statement.Methods)
            {
                var function = new LoxFunction(method, environment, LoxClass.IsInitializer(method));
                methods.Add(method.Name.Lexeme, function);
            }

            var loxClass = new LoxClass(statement.Name.Lexeme, (LoxClass)superclass, methods);

            environment.Assign(statement.Name, loxClass);

            return null;
        }

        public object VisitContinueStatement(ContinueStatement statement)
        {
            throw new LoxContinueException();
        }

        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            var value = Evaluate(statement.Expression);

            RaiseOut(value, true);

            return null;
        }

        public object VisitFunctionStatement(FunctionStatement statement)
        {
            var function = new LoxFunction(statement, environment);

            environment.Define(statement.Name, function);

            return null;
        }

        public object VisitIfStatement(IfStatement statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                Execute(statement.ThenBranch);
            }
            else if (statement.ElseBranch != null)
            {
                Execute(statement.ElseBranch);
            }

            return null;
        }

        public object VisitLoopStatement(LoopStatement statement)
        {
            Execute(statement.Initializer);

            while (IsTruthy(Evaluate(statement.Condition)))
            {
                try
                {
                    Execute(statement.Body);
                }
                catch (LoxBreakException) { break; }
                catch (LoxContinueException) { continue; }
                finally
                {
                    Evaluate(statement.Increment);
                }
            }

            return null;
        }

        public object VisitReturnStatement(ReturnStatement statement)
        {
            var value = Evaluate(statement.Value);

            throw new LoxReturn(value);
        }

        public object VisitVariableStatement(VariableStatement statement)
        {
            var value = Evaluate(statement.Initializer);

            environment.Define(statement.Name, value);

            return null;
        }
        #endregion
    }
}
