using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLGotoInstruction : LLInstruction
    {
        public static LLGotoInstruction Create(LLFunction pFunction, LLLabel pTargetLabel)
        {
            LLGotoInstruction instruction = new LLGotoInstruction(pFunction);
            instruction.mTargetLabel = pTargetLabel;
            return instruction;
        }

        private LLLabel mTargetLabel = null;

        private LLGotoInstruction(LLFunction pFunction) : base(pFunction) { }

        public override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            return string.Format("br label %{0}", mTargetLabel);
        }
    }
}
