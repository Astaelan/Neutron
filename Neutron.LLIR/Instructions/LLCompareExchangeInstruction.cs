using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public enum LLCompareExchangeOrdering
    {
        unordered,
        monotonic,
        aquire,
        release,
        acq_rel,
        seq_cst
    }

    public sealed class LLCompareExchangeInstruction : LLInstruction
    {
        public static LLCompareExchangeInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pPointerSource, LLLocation pComparedSource, LLLocation pNewSource, LLCompareExchangeOrdering pOrdering)
        {
            LLCompareExchangeInstruction instruction = new LLCompareExchangeInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mPointerSource = pPointerSource;
            instruction.mComparedSource = pComparedSource;
            instruction.mNewSource = pNewSource;
            instruction.mOrdering = pOrdering;
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mPointerSource = null;
        private LLLocation mComparedSource = null;
        private LLLocation mNewSource = null;
        private LLCompareExchangeOrdering mOrdering = LLCompareExchangeOrdering.unordered;

        private LLCompareExchangeInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            return string.Format("{0} = cmpxchg {1} {2}, {3} {4}, {5} {6} {7}", mDestination, mPointerSource.Type, mPointerSource, mComparedSource.Type, mComparedSource, mNewSource.Type, mNewSource, mOrdering);
        }
    }
}
