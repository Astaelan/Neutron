using System.Reflection;

namespace System
{
    public sealed class RuntimeType : Type
    {
        private RuntimeTypeHandle mHandle;

        internal RuntimeType() { }
        private RuntimeType(RuntimeTypeHandle pHandle) { mHandle = pHandle; }

        public override string Name { get { return "???"; } }
    }
}
