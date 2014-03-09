using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Type : MemberInfo
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern Type GetTypeFromHandle(RuntimeTypeHandle pHandle);

        protected Type() { }

        public abstract string Namespace { get; }
    }
}
