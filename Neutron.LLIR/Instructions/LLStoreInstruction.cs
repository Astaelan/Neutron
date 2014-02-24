using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLStoreInstruction : LLInstruction
    {
        public static LLStoreInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLStoreInstruction instruction = new LLStoreInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mSource = null;

        private LLStoreInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("store {0} {1}, {2} {3}", mSource.Type, mSource, mDestination.Type, mDestination);
        }
    }
}
