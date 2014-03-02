using System.Runtime.CompilerServices;

namespace System
{
    public class Object
    {
        public Object() { }

        ~Object() { }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool InternalEquals(object objA, object objB);
    }
}
