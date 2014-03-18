using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    internal sealed unsafe class RuntimeType : Type
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static unsafe extern RuntimeTypeData* InternalGetRuntimeTypeData(RuntimeTypeHandle pHandle);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static unsafe extern sbyte* InternalGetRuntimeTypeDataString(int pOffset);

        private RuntimeTypeHandle mHandle;
        private RuntimeTypeData* mData;

        //private RuntimeType() { }
        internal RuntimeType(RuntimeTypeHandle pHandle)
        {
            mHandle = pHandle;
            mData = InternalGetRuntimeTypeData(pHandle);
        }

        public override string ToString() { return Namespace + "." + Name; }

        public override string Name { get { return new string(InternalGetRuntimeTypeDataString(mData->NameOffset)); } }
        public override string Namespace { get { return new string(InternalGetRuntimeTypeDataString(mData->NamespaceOffset)); } }

    }
}
