using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLTruncateInstruction : LLConversionInstruction
    {
        public static LLTruncateInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLTruncateInstruction instruction = new LLTruncateInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLTruncateInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mSource.Type.Primitive)
            {
                case LLPrimitive.Signed:
                case LLPrimitive.Unsigned: operation = "trunc"; break;
                case LLPrimitive.Float: operation = "fptrunc"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3} to {4}", mDestination, operation, mSource.Type, mSource, mDestination.Type);
        }
    }
}
