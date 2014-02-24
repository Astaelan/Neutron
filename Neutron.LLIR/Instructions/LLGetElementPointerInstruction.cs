using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Instructions
{
    public sealed class LLGetElementPointerInstruction : LLInstruction
    {
        public static LLGetElementPointerInstruction Create(LLFunction pFunction, LLLocation pDestination, LLLocation pPointerSource, params LLLocation[] pIndexSources)
        {
            LLGetElementPointerInstruction instruction = new LLGetElementPointerInstruction(pFunction);
            instruction.mDestination = pDestination;
            instruction.mPointerSource = pPointerSource;
            instruction.mIndexSources = new List<LLLocation>(pIndexSources);
            return instruction;
        }

        private LLLocation mDestination = null;
        private LLLocation mPointerSource = null;
        private List<LLLocation> mIndexSources = null;

        private LLGetElementPointerInstruction(LLFunction pFunction) : base(pFunction) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} = getelementptr {1} {2}", mDestination, mPointerSource.Type, mPointerSource);
            for (int index = 0; index < mIndexSources.Count; ++index)
                sb.AppendFormat(", {0} {1}", mIndexSources[index].Type, mIndexSources[index]);
            return sb.ToString();
        }
    }
}
