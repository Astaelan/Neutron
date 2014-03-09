using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RuntimeTypeData
    {
        public const int IsValueFlag = 1 << 0;

        public int Handle;
        public int Flags;
        public int Size;
        public int Name;
        public int Namespace;
        public int VTable;
    }
}
