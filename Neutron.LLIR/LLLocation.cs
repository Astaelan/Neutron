using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public abstract class LLLocation
    {
        private LLType mType = null;

        protected LLLocation(LLType pType)
        {
            mType = pType;
        }

        public LLType Type { get { return mType; } }

        public virtual LLLocation Load(LLInstructionBlock pBlock) { throw new NotSupportedException(); }

        public virtual void Store(LLInstructionBlock pBlock, LLLocation pSource) { throw new NotSupportedException(); }
    }
}
