using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLBitwiseNegateInstruction : HLInstruction
    {
        public static HLBitwiseNegateInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pSource)
        {
            HLBitwiseNegateInstruction instruction = new HLBitwiseNegateInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mSource = null;

        private HLBitwiseNegateInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = -{1}", mDestination, mSource); }
    }
}
