using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLDivideInstruction : LLBinaryInstruction
    {
        public static LLDivideInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLDivideInstruction instruction = new LLDivideInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLDivideInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mDestination.Type.Primitive)
            {
                case LLPrimitive.Signed: operation = "sdiv"; break;
                case LLPrimitive.Unsigned: operation = "udiv"; break;
                case LLPrimitive.Float: operation = "fdiv"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3}, {4}", mDestination, operation, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
