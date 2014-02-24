using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLLabelInstruction : HLInstruction
    {
        public static HLLabelInstruction Create(HLMethod pMethod, HLLabel pLabel)
        {
            HLLabelInstruction instruction = new HLLabelInstruction(pMethod);
            instruction.mLabel = pLabel;
            return instruction;
        }

        private HLLabel mLabel = null;

        private HLLabelInstruction(HLMethod pMethod) : base(pMethod) { }

        internal override bool AffectsTermination { get { return false; } }

        public override string ToString()
        {
            return string.Format("{0}:", mLabel);
        }

        internal override void Transform(LLFunction pFunction)
        {
        }
    }
}
