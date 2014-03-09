using System.Runtime.CompilerServices;

namespace System
{
    public class Object
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern RuntimeTypeHandle InternalGetRuntimeTypeHandle(object pObject);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool InternalEquals(object pObjectA, object pObjectB);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int InternalHashCode(object pObject);

        public static bool Equals(object pObjectA, object pObjectB) { return (pObjectA == pObjectB) || (pObjectA != null && pObjectB != null && pObjectA.Equals(pObjectB)); }

        public static bool ReferenceEquals(object pObjectA, object pObjectB) { return pObjectA == pObjectB; }

        public Object() { }
        ~Object() { }

        public Type GetType() { return new RuntimeType(InternalGetRuntimeTypeHandle(this)); }

        [MethodImpl(MethodImplOptions.InternalCall)]
        protected extern object MemberwiseClone();

        public virtual bool Equals(object pObject) { return InternalEquals(this, pObject); }

        public virtual int GetHashCode() { return InternalHashCode(this); }

        public virtual string ToString() { return GetType().ToString(); }
    }
}
