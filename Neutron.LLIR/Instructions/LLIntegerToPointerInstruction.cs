using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLIntegerToPointerInstruction : LLConversionInstruction
    {
        public static LLIntegerToPointerInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLIntegerToPointerInstruction instruction = new LLIntegerToPointerInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLIntegerToPointerInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = inttoptr {1} {2} to {3}", mDestination, mSource.Type, mSource, mDestination.Type);
        }
    }
}
