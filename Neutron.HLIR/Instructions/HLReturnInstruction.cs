using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLReturnInstruction : HLInstruction
    {
        public static HLReturnInstruction Create(HLMethod pMethod, HLLocation pSource)
        {
            HLReturnInstruction instruction = new HLReturnInstruction(pMethod);
            instruction.mSource = pSource;
            return instruction;
        }

        private HLLocation mSource = null;

        private HLReturnInstruction(HLMethod pMethod) : base(pMethod) { }

        internal override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("return");
            if (mSource != null) sb.AppendFormat(" {0}", mSource);
            return sb.ToString();
        }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationSource = null;
            if (mSource != null) locationSource = mSource.Load(pFunction);
            pFunction.CurrentBlock.EmitReturn(locationSource);
        }
    }
}
