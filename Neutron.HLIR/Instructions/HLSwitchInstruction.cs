using Neutron.LLIR;
using Neutron.LLIR.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLSwitchInstruction : HLInstruction
    {
        public static HLSwitchInstruction Create(HLMethod pMethod, HLLocation pConditionSource, HLLabel pDefaultLabel, List<Tuple<HLLiteralLocation, HLLabel>> pCases)
        {
            HLSwitchInstruction instruction = new HLSwitchInstruction(pMethod);
            instruction.mConditionSource = pConditionSource;
            instruction.mDefaultLabel = pDefaultLabel;
            instruction.mCases = new List<Tuple<HLLiteralLocation, HLLabel>>(pCases);
            return instruction;
        }

        private HLLocation mConditionSource = null;
        private HLLabel mDefaultLabel = null;
        private List<Tuple<HLLiteralLocation, HLLabel>> mCases = null;

        private HLSwitchInstruction(HLMethod pMethod) : base(pMethod) { }

        internal override bool IsTerminator { get { return true; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("switch ({0}) default: goto {1}", mConditionSource, mDefaultLabel);
            foreach (Tuple<HLLiteralLocation, HLLabel> tupleCase in mCases)
                sb.AppendFormat(" {0}: goto {1}", tupleCase.Item1.LiteralAsString, tupleCase.Item2);
            return sb.ToString();
        }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationCondition = mConditionSource.Load(pFunction);
            List<LLSwitchCase> cases = new List<LLSwitchCase>();
            foreach (Tuple<HLLiteralLocation, HLLabel> tupleCase in mCases)
                cases.Add(LLSwitchCase.Create(LLLiteral.Create(tupleCase.Item1.Type.LLType, tupleCase.Item1.LiteralAsString), pFunction.Labels.GetByIdentifier(tupleCase.Item2.Identifier)));
            pFunction.CurrentBlock.EmitSwitch(locationCondition, pFunction.Labels.GetByIdentifier(mDefaultLabel.Identifier), cases);
        }
    }
}
