using System.Runtime.CompilerServices;
namespace System
{
    public abstract class Enum : ValueType
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void InternalGetEnumData(RuntimeTypeHandle pHandle, int pIndex, ref ulong pValue, ref int pNameOffset);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int InternalGetEnumIndex(RuntimeTypeHandle pHandle, ulong pValue);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern int InternalGetEnumName(RuntimeTypeHandle pHandle, int pIndex);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong InternalGetEnumValue(object pObject);

        protected Enum() { }

        public unsafe override string ToString()
        {
            RuntimeTypeHandle handle = InternalGetRuntimeTypeHandle(this);
            ulong value = InternalGetEnumValue(this);
            int index = InternalGetEnumIndex(handle, value);
            if (index >= 0)
            {
                int offsetName = InternalGetEnumName(handle, index);
                return new string(RuntimeType.InternalGetRuntimeTypeDataString(offsetName));
            }
            return base.ToString(); // return value.ToString();
        }
    }
}
