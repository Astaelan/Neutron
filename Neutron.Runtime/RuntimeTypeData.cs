using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RuntimeTypeData
    {
        public const int IsValueFlag = 1 << 0;
        public const int IsEnumFlag = 1 << 1;

        public int Handle;
        public int Flags;
        public int Size;
        public int NameOffset;
        public int NamespaceOffset;
        public int VTableOffset;

        public int EnumOffset;
        public int EnumCount;
    }
}
