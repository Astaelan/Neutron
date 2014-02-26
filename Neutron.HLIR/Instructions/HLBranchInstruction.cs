using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLBranchInstruction : HLInstruction
    {
        public static HLBranchInstruction Create(HLMethod pMethod, HLLocation pConditionSource, HLLabel pTrueLabel, HLLabel pFalseLabel)
        {
            HLBranchInstruction instruction = new HLBranchInstruction(pMethod);
            instruction.mConditionSource = pConditionSource;
            instruction.mTrueLabel = pTrueLabel;
            instruction.mFalseLabel = pFalseLabel;
            return instruction;
        }

        private HLLocation mConditionSource = null;
        private HLLabel mTrueLabel = null;
        private HLLabel mFalseLabel = null;

        private HLBranchInstruction(HLMethod pMethod) : base(pMethod) { }

        internal override bool IsTerminator { get { return true; } }

        public override string ToString() { return string.Format("if ({0}) goto {1} else goto {2}", mConditionSource, mTrueLabel, mFalseLabel); }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationCondition = mConditionSource.Load(pFunction);
            locationCondition = pFunction.CurrentBlock.EmitConversion(locationCondition, LLModule.BooleanType);
            pFunction.CurrentBlock.EmitBranch(locationCondition, pFunction.Labels.GetByIdentifier(mTrueLabel.Identifier), pFunction.Labels.GetByIdentifier(mFalseLabel.Identifier));
        }
    }
}
