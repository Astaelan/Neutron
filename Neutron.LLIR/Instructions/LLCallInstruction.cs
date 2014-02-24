using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLCallInstruction : LLInstruction
    {
        public static LLCallInstruction Create(LLFunction pFunction, LLLocation pReturnDestination, LLLocation pFunctionSource, List<LLLocation> pParameterSources)
        {
            LLCallInstruction instruction = new LLCallInstruction(pFunction);
            instruction.mReturnDestination = pReturnDestination;
            instruction.mFunctionSource = pFunctionSource;
            instruction.mParameterSources = new List<LLLocation>(pParameterSources);
            return instruction;
        }

        private LLLocation mReturnDestination = null;
        private LLLocation mFunctionSource = null;
        private List<LLLocation> mParameterSources = null;

        private LLCallInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (mReturnDestination != null) sb.AppendFormat("{0} = ", mReturnDestination);
            sb.AppendFormat("call {0} {1}(", mReturnDestination == null ? LLModule.VoidType : mReturnDestination.Type, mFunctionSource);
            for (int index = 0; index < mParameterSources.Count; ++index)
            {
                LLLocation parameterSource = mParameterSources[index];
                if (index > 0) sb.Append(", ");
                sb.AppendFormat("{0} {1}", parameterSource.Type, parameterSource);
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
