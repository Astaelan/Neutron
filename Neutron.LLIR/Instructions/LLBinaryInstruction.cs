using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public abstract class LLBinaryInstruction : LLInstruction
    {
        protected LLLocation mDestination = null;
        protected LLLocation mLeftSource = null;
        protected LLLocation mRightSource = null;

        protected LLBinaryInstruction(LLFunction pFunction) : base(pFunction) { }
    }
}
