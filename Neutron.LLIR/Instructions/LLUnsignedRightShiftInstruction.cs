using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLUnsignedRightShiftInstruction : LLBinaryInstruction
    {
        public static LLUnsignedRightShiftInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLUnsignedRightShiftInstruction instruction = new LLUnsignedRightShiftInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLUnsignedRightShiftInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = lshr {1} {2}, {3}", mDestination, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
