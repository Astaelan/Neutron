using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLPointerToIntegerInstruction : LLConversionInstruction
    {
        public static LLPointerToIntegerInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLPointerToIntegerInstruction instruction = new LLPointerToIntegerInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLPointerToIntegerInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = ptrtoint {1} {2} to {3}", mDestination, mSource.Type, mSource, mDestination.Type);
        }
    }
}
