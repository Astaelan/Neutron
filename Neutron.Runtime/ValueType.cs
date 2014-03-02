using System.Runtime.CompilerServices;
namespace System
{
    public abstract class ValueType
    {
        protected ValueType() { }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern new bool InternalEquals(object objA, object objB);
    }
}
