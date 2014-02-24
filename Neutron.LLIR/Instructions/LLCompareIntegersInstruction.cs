using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public enum LLCompareIntegersCondition
    {
        eq,
        ne,
        ugt,
        uge,
        ult,
        ule,
        sgt,
        sge,
        slt,
        sle,
    }

    public sealed class LLCompareIntegersInstruction : LLInstruction
    {
        public static LLCompareIntegersInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource, LLCompareIntegersCondition pCondition)
        {
            LLCompareIntegersInstruction instruction = new LLCompareIntegersInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            instruction.mCondition = pCondition;
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mLeftSource = null;
        private LLLocation mRightSource = null;
        private LLCompareIntegersCondition mCondition = LLCompareIntegersCondition.eq;

        private LLCompareIntegersInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = icmp {1} {2} {3}, {4}", mDestination, mCondition, mLeftSource.Type, mLeftSource, mRightSource);
        }
    }
}
