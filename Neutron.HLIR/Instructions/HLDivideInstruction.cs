using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLDivideInstruction : HLInstruction
    {
        public static HLDivideInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource)
        {
            HLDivideInstruction instruction = new HLDivideInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mLeftOperandSource = pLeftOperandSource;
            instruction.mRightOperandSource = pRightOperandSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mLeftOperandSource = null;
        private HLLocation mRightOperandSource = null;

        private HLDivideInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = {1} / {2}", mDestination, mLeftOperandSource, mRightOperandSource); }
    }
}
