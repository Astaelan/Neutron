using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public abstract class LLInstruction
    {
        private LLFunction mFunction = null;

        protected LLInstruction(LLFunction pFunction)
        {
            mFunction = pFunction;
        }

        public virtual bool IsTerminator { get { return false; } }
        public virtual bool AffectsTermination { get { return true; } }
    }
}
