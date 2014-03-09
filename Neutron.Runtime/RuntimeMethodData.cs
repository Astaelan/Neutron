using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RuntimeMethodData
    {
        public int Handle;
        public int Flags;
        public int Name;
    }
}
