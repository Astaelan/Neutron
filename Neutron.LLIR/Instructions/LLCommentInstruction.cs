using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLCommentInstruction : LLInstruction
    {
        public static LLCommentInstruction Create(LLFunction pFunction, string pComment)
        {
            LLCommentInstruction instruction = new LLCommentInstruction(pFunction);
            instruction.mComment = pComment;
            return instruction;
        }

        private string mComment = null;

        private LLCommentInstruction(LLFunction pFunction) : base(pFunction) { }

        public override bool AffectsTermination { get { return false; } }

        public override string ToString()
        {
            return string.Format("; {0}", mComment);
        }
    }
}
