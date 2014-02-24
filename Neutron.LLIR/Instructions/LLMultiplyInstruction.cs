using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLMultiplyInstruction : LLBinaryInstruction
    {
        public static LLMultiplyInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLMultiplyInstruction instruction = new LLMultiplyInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLMultiplyInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mDestination.Type.Primitive)
            {
                case LLPrimitive.Signed:
                case LLPrimitive.Unsigned: operation = "mul"; break;
                case LLPrimitive.Float: operation = "fmul"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3}, {4}", mDestination, operation, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
