using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLAllocateInstruction : LLInstruction
    {
        public static LLAllocateInstruction Create(LLFunction pFunction, LLLocation pDestination)
        {
            LLAllocateInstruction instruction = new LLAllocateInstruction(pFunction);
            instruction.mDestination = pDestination;
            return instruction;
        }

        private LLLocation mDestination = null;

        private LLAllocateInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = alloca {1}", mDestination, mDestination.Type.PointerDepthMinusOne);
        }
    }
}
