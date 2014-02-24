using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public abstract class LLConversionInstruction : LLInstruction
    {
        protected LLLocation mDestination = null;
        protected LLLocation mSource = null;

        protected LLConversionInstruction(LLFunction pFunction) : base(pFunction) { }
    }
}
