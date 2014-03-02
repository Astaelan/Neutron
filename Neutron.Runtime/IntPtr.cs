namespace System
{
    public struct IntPtr
    {
        public static readonly IntPtr Zero;

#pragma warning disable 0649
        private unsafe void* mValue;
#pragma warning restore 0649

        public unsafe IntPtr(int pValue) { mValue = (void*)pValue; }
        public unsafe IntPtr(long pValue) { mValue = (void*)pValue; }
        public unsafe IntPtr(void* pValue) { mValue = pValue; }
    }
}
