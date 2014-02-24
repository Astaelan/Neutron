using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLBitwiseXorInstruction : HLInstruction
    {
        public static HLBitwiseXorInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource)
        {
            HLBitwiseXorInstruction instruction = new HLBitwiseXorInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mLeftOperandSource = pLeftOperandSource;
            instruction.mRightOperandSource = pRightOperandSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mLeftOperandSource = null;
        private HLLocation mRightOperandSource = null;

        private HLBitwiseXorInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = {1} ^ {2}", mDestination, mLeftOperandSource, mRightOperandSource); }
    }
}
