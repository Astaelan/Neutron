namespace System
{
    public struct IntPtr
    {
        public static readonly IntPtr Zero;

        public static unsafe bool operator ==(IntPtr pValueA, IntPtr pValueB) { return pValueA.mValue == pValueB.mValue; }
        public static unsafe bool operator !=(IntPtr pValueA, IntPtr pValueB) { return pValueA.mValue != pValueB.mValue; }

#pragma warning disable 0649
        private unsafe void* mValue;
#pragma warning restore 0649

        public unsafe IntPtr(int pValue) { mValue = (void*)pValue; }
        public unsafe IntPtr(long pValue) { mValue = (void*)pValue; }
        public unsafe IntPtr(void* pValue) { mValue = pValue; }

        public override unsafe bool Equals(object pObject) { return (pObject is IntPtr) && mValue == ((IntPtr)pObject).mValue; }

        public override unsafe int GetHashCode() { return (int)mValue; }
    }
}
