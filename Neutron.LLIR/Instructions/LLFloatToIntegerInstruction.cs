using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLFloatToIntegerInstruction : LLConversionInstruction
    {
        public static LLFloatToIntegerInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pSource)
        {
            LLFloatToIntegerInstruction instruction = new LLFloatToIntegerInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private LLFloatToIntegerInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            string operation = null;
            switch (mDestination.Type.Primitive)
            {
                case LLPrimitive.Signed: operation = "fptosi"; break;
                case LLPrimitive.Unsigned: operation = "fptoui"; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3} to {4}", mDestination, operation, mSource.Type, mSource, mDestination.Type);
        }
    }
}
