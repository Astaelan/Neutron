using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLLabelInstruction : LLInstruction
    {
        public static LLLabelInstruction Create(LLFunction pFunction, LLLabel pLabel)
        {
            LLLabelInstruction instruction = new LLLabelInstruction(pFunction);
            instruction.mLabel = pLabel;
            return instruction;
        }

        private LLLabel mLabel = null;

        private LLLabelInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0}:", mLabel);
        }
    }
}
