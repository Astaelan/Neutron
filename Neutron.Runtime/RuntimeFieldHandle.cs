namespace System
{
    public struct RuntimeFieldHandle
    {
        private IntPtr mValue;

        internal unsafe RuntimeFieldHandle(void* pValue) { mValue = new IntPtr(pValue); }

        public IntPtr Value { get { return mValue; } }
    }
}
