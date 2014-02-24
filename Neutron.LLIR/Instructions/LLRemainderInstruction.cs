using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLRemainderInstruction : LLBinaryInstruction
    {
        public static LLRemainderInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource)
        {
            LLRemainderInstruction instruction = new LLRemainderInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mLeftSource = pLeftSource;
            instruction.mRightSource = pRightSource;
            return instruction;
        }

        private LLRemainderInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mDestination.Type.Primitive)
            {
                case LLPrimitive.Signed: operation = "srem"; break;
                case LLPrimitive.Unsigned: operation = "urem"; break;
                case LLPrimitive.Float: operation = "frem"; break;
            }
            return string.Format("{0} = {1} {2} {3}, {4}", mDestination, operation, mDestination.Type, mLeftSource, mRightSource);
        }
    }
}
