namespace System.Runtime.CompilerServices
{
    [Flags]
    public enum MethodImplOptions
    {
        Unmanaged = 0x0004,
        NoInlining = 0x0008,
        ForwardRef = 0x0010,
        Synchronized = 0x0020,
        PreserveSig = 0x0080,
        InternalCall = 0x1000,
    }
}
