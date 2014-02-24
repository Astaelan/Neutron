using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLReturnInstruction : LLInstruction
    {
        public static LLReturnInstruction Create(LLFunction pFunction, LLLocation pSource)
        {
            LLReturnInstruction instruction = new LLReturnInstruction(pFunction);
            instruction.mSource = pSource;
            return instruction;
        }

        private LLLocation mSource = null;

        private LLReturnInstruction(LLFunction pFunction) : base(pFunction) { }

        public override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ret");
            if (mSource == null) sb.AppendFormat(" {0}", LLModule.VoidType);
            else sb.AppendFormat(" {0} {1}", mSource.Type, mSource);
            return sb.ToString();
        }
    }
}
