namespace System
{
    public struct RuntimeTypeHandle
    {
        private IntPtr mValue;

        internal unsafe RuntimeTypeHandle(void* pValue) { mValue = new IntPtr(pValue); }

        public IntPtr Value { get { return mValue; } }
    }
}
