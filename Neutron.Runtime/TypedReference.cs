using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TypedReference
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe object InternalToObject(TypedReference* pReference);

        private RuntimeTypeHandle mType;
        private IntPtr mValue;

        public override bool Equals(object pObject)
        {
            throw new NotSupportedException();
        }
        public override int GetHashCode()
        {
            if (mType.Value == IntPtr.Zero) return 0;
            return Type.GetTypeFromHandle(mType).GetHashCode();
        }
    }
}
