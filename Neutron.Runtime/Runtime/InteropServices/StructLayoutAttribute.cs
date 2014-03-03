namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public sealed class StructLayoutAttribute : Attribute
    {
        private LayoutKind mValue;
        public int Pack;
        public int Size;

        public StructLayoutAttribute(LayoutKind pValue) { mValue = pValue; }

        public LayoutKind Value { get { return mValue; } }
    }
}
