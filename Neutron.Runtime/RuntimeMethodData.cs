using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RuntimeMethodData
    {
        public const int IsStaticFlag = 1 << 0;

        public int Handle;
        public int Flags;
        public int Name;
    }
}
