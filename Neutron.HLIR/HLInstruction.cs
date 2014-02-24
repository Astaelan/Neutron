using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public abstract class HLInstruction
    {
        private HLMethod mMethod = null;

        protected HLInstruction(HLMethod pMethod)
        {
            mMethod = pMethod;
        }

        internal virtual bool IsTerminator { get { return false; } }
        internal virtual bool AffectsTermination { get { return true; } }

        internal virtual void Transform(LLFunction pFunction) { throw new NotImplementedException(); }
    }
}
