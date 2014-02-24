using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLLoadInstruction : LLInstruction
    {
        public static LLLoadInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLLoadInstruction instruction = new LLLoadInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mSource = null;

        private LLLoadInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = load {1} {2}", mDestination, mSource.Type, mSource);
        }
    }
}
