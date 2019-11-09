namespace LoxFramework.AST
{
    abstract class Expression { }
    
    class Binary : Expression
    {
        public readonly Expression Left;
        public readonly Token Operator;
        public readonly Expression Right;
        
        public Binary(Expression left, Token op, Expression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }
    
    class Grouping : Expression
    {
        public readonly Expression Expression;
        
        public Grouping(Expression expression)
        {
            Expression = expression;
        }
    }
    
    class Literal : Expression
    {
        public readonly object Value;
        
        public Literal(object value)
        {
            Value = value;
        }
    }
    
    class Unary : Expression
    {
        public readonly Token Operator;
        public readonly Expression Right;
        
        public Unary(Token op, Expression right)
        {
            Operator = op;
            Right = right;
        }
    }
}
