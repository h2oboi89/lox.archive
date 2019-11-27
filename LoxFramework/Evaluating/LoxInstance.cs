namespace LoxFramework.Evaluating
{
    class LoxInstance
    {
        private LoxClass loxClass;

        public LoxInstance(LoxClass loxClass)
        {
            this.loxClass = loxClass;
        }

        public override string ToString()
        {
            return $"{loxClass.Name} instance";
        }
    }
}
