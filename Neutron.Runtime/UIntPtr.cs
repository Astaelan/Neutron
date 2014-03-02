namespace System
{
    public struct UIntPtr
    {
#pragma warning disable 0649
        private unsafe void* mValue;
#pragma warning restore 0649

        public unsafe UIntPtr(uint pValue) { mValue = (void*)pValue; }
        public unsafe UIntPtr(ulong pValue) { mValue = (void*)pValue; }
        public unsafe UIntPtr(void* pValue) { mValue = pValue; }
    }
}
