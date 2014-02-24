using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public abstract class HLLocation
    {
        protected HLLocation(HLType pType) { mType = pType; }

        private HLType mType = null;
        public HLType Type { get { return mType; } }

        internal virtual HLLocation AddressOf() { throw new NotSupportedException(); }

        internal virtual LLLocation Load(LLFunction pFunction) { throw new NotSupportedException(); }

        internal virtual void Store(LLFunction pFunction, LLLocation pSource) { throw new NotSupportedException(); }
    }
}
