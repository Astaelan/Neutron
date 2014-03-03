namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
    public sealed class MethodImplAttribute : Attribute
    {
        private MethodImplOptions mOptions;

        public MethodImplAttribute() { }

        public MethodImplAttribute(MethodImplOptions pOptions) { mOptions = pOptions; }

        public MethodImplOptions Value { get { return mOptions; } }
    }
}
