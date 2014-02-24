using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public enum HLCompareType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public sealed class HLCompareInstruction : HLInstruction
    {
        public static HLCompareInstruction Create(HLMethod pMethod, HLCompareType pCompareType, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource)
        {
            HLCompareInstruction instruction = new HLCompareInstruction(pMethod);
            instruction.mCompareType = pCompareType;
            instruction.mDestination = pDestination;
            instruction.mLeftOperandSource = pLeftOperandSource;
            instruction.mRightOperandSource = pRightOperandSource;
            return instruction;
        }

        private HLCompareType mCompareType = HLCompareType.Equal;
        private HLLocation mDestination = null;
        private HLLocation mLeftOperandSource = null;
        private HLLocation mRightOperandSource = null;

        private HLCompareInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            string operation = "";
            switch (mCompareType)
            {
                case HLCompareType.Equal: operation = "=="; break;
                case HLCompareType.NotEqual: operation = "!="; break;
                case HLCompareType.GreaterThan: operation = ">"; break;
                case HLCompareType.GreaterThanOrEqual: operation = ">="; break;
                case HLCompareType.LessThan: operation = "<"; break;
                case HLCompareType.LessThanOrEqual: operation = "<="; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3}", mDestination, mLeftOperandSource, operation, mRightOperandSource);
        }
    }
}
