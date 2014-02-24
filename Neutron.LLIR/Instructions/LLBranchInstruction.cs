using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLBranchInstruction : LLInstruction
    {
        public static LLBranchInstruction Create(LLFunction pFunction, LLLocation pConditionSource, LLLabel pTrueTargetLabel, LLLabel pFalseTargetLabel)
        {
            LLBranchInstruction instruction = new LLBranchInstruction(pFunction);
            instruction.mConditionSource = pConditionSource;
            instruction.mTrueTargetLabel = pTrueTargetLabel;
            instruction.mFalseTargetLabel = pFalseTargetLabel;
            return instruction;
        }

        private LLLocation mConditionSource = null;
        private LLLabel mTrueTargetLabel = null;
        private LLLabel mFalseTargetLabel = null;

        private LLBranchInstruction(LLFunction pFunction) : base(pFunction) { }

        public override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            return string.Format("br {0} {1}, label %{2}, label %{3}", mConditionSource.Type, mConditionSource, mTrueTargetLabel, mFalseTargetLabel);
        }
    }
}
