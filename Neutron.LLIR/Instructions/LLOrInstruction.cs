using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLOrInstruction : LLBinaryInstruction
    {
        public static LLOrInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLOrInstruction instruction = new LLOrInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLOrInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = or {1} {2}, {3}", mDestination, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
