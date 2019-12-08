namespace Lox.Runtime.Globals
{
    class LoxBuiltInFunction : LoxCallable
    {
        public override string ToString()
        {
            return "<function native>";
        }
    }
}
