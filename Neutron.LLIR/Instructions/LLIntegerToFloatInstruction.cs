using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLIntegerToFloatInstruction : LLConversionInstruction
    {
        public static LLIntegerToFloatInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLIntegerToFloatInstruction instruction = new LLIntegerToFloatInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLIntegerToFloatInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mSource.Type.Primitive)
            {
                case LLPrimitive.Signed: operation = "sitofp"; break;
                case LLPrimitive.Unsigned: operation = "uitofp"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3} to {4}", mDestination, operation, mSource.Type, mSource, mDestination.Type);
        }
    }
}
