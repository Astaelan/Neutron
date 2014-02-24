using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public enum LLCompareFloatsCondition
    {
        oeq,
        ogt,
        oge,
        olt,
        ole,
        one,
        ord,
        ueq,
        ugt,
        uge,
        ult,
        ule,
        une,
        uno,
    }

    public sealed class LLCompareFloatsInstruction : LLInstruction
    {
        public static LLCompareFloatsInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource, LLCompareFloatsCondition pCondition)
        {
            LLCompareFloatsInstruction instruction = new LLCompareFloatsInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            instruction.mCondition = pCondition;
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mLeftSource = null;
        private LLLocation mRightSource = null;
        private LLCompareFloatsCondition mCondition = LLCompareFloatsCondition.oeq;

        private LLCompareFloatsInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = fcmp {1} {2} {3}, {4}", mDestination, mCondition, mLeftSource.Type, mLeftSource, mRightSource);
        }
    }
}
