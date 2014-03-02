namespace System
{
    public struct RuntimeMethodHandle
    {
        private IntPtr mValue;

        internal unsafe RuntimeMethodHandle(void* pValue) { mValue = new IntPtr(pValue); }
        internal unsafe RuntimeMethodHandle(IntPtr pValue) { mValue = pValue; }

        public IntPtr Value { get { return mValue; } }
    }
}
