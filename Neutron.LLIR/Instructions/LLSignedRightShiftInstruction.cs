using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLSignedRightShiftInstruction : LLBinaryInstruction
    {
        public static LLSignedRightShiftInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLSignedRightShiftInstruction instruction = new LLSignedRightShiftInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLSignedRightShiftInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = ashr {1} {2}, {3}", mDestination, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
