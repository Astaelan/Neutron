using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLGotoInstruction : HLInstruction
    {
        public static HLGotoInstruction Create(HLMethod pMethod, HLLabel pTargetLabel)
        {
            HLGotoInstruction instruction = new HLGotoInstruction(pMethod);
            instruction.mTargetLabel = pTargetLabel;
            return instruction;
        }

        private HLLabel mTargetLabel = null;

        private HLGotoInstruction(HLMethod pMethod) : base(pMethod) { }

        internal override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            return string.Format("goto {0}", mTargetLabel);
        }

        internal override void Transform(LLFunction pFunction)
        {
            pFunction.CurrentBlock.EmitGoto(pFunction.Labels.GetByIdentifier(mTargetLabel.Identifier));
        }
    }
}
