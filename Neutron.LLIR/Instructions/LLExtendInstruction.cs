using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLExtendInstruction : LLConversionInstruction
    {
        public static LLExtendInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLExtendInstruction instruction = new LLExtendInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLExtendInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mSource.Type.Primitive)
            {
                case LLPrimitive.Signed: operation = "sext"; break;
                case LLPrimitive.Unsigned: operation = "zext"; break;
                case LLPrimitive.Float: operation = "fpext"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3} to {4}", mDestination, operation, mSource.Type, mSource, mDestination.Type);
        }
    }
}
