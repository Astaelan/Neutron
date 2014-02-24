using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLAndInstruction : LLBinaryInstruction
    {
        public static LLAndInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLAndInstruction instruction = new LLAndInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLAndInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = and {1} {2}, {3}", mDestination, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
