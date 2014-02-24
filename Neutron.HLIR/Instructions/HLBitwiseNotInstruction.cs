using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLBitwiseNotInstruction : HLInstruction
    {
        public static HLBitwiseNotInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pSource)
        {
            HLBitwiseNotInstruction instruction = new HLBitwiseNotInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mSource = null;

        private HLBitwiseNotInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = ~{1}", mDestination, mSource); }
    }
}
