using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLSwitchCase
    {
        public static LLSwitchCase Create(LLLiteral pLiteral, LLLabel pLabel)
        {
            LLSwitchCase switchCase = new LLSwitchCase();
            switchCase.mLiteral = pLiteral;
            switchCase.mLabel = pLabel;
            return switchCase;
        }

        private LLLiteral mLiteral = null;
        private LLLabel mLabel = null;

        private LLSwitchCase() { }

        public LLLiteral Literal { get { return mLiteral; } }

        public LLLabel Label { get { return mLabel; } }

        public override string ToString()
        {
            return string.Format("{0} {1}, label %{2}", mLiteral.Type, mLiteral, mLabel);
        }
    }

    public sealed class LLSwitchInstruction : LLInstruction
    {
        public static LLSwitchInstruction Create(LLFunction pFunction, LLLocation pConditionSource, LLLabel pDefaultTargetLabel, List<LLSwitchCase> pCases)
        {
            LLSwitchInstruction instruction = new LLSwitchInstruction(pFunction);
            instruction.mConditionSource = pConditionSource;
            instruction.mDefaultTargetLabel = pDefaultTargetLabel;
            instruction.mCases = new List<LLSwitchCase>(pCases);
            return instruction;
        }

        private LLLocation mConditionSource = null;
        private LLLabel mDefaultTargetLabel = null;
        private List<LLSwitchCase> mCases = null;

        private LLSwitchInstruction(LLFunction pFunction) : base(pFunction) { }

        public override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("switch {0} {1}, label %{2} [ ", mConditionSource.Type, mConditionSource, mDefaultTargetLabel);
            foreach (LLSwitchCase switchCase in mCases) sb.AppendFormat("{0} ", switchCase);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
