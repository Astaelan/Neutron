using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLBitcastInstruction : LLConversionInstruction
    {
        public static LLBitcastInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLBitcastInstruction instruction = new LLBitcastInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLBitcastInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = bitcast {1} {2} to {3}", mDestination, mSource.Type, mSource, mDestination.Type);
        }
    }
}
