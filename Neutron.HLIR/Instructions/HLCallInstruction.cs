using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLCallInstruction : HLInstruction
    {
        public static HLCallInstruction Create(HLMethod pMethod, HLMethod pCalledMethod, HLLocation pReturnDestination, List<HLLocation> pParameterSources)
        {
            HLCallInstruction instruction = new HLCallInstruction(pMethod);
            instruction.mCalledMethod = pCalledMethod;
            instruction.mReturnDestination = pReturnDestination;
            instruction.mParameterSources = new List<HLLocation>(pParameterSources);
            return instruction;
        }

        private HLMethod mCalledMethod = null;
        private HLLocation mReturnDestination = null;
        private List<HLLocation> mParameterSources = null;

        private HLCallInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (mReturnDestination != null) sb.AppendFormat("{0} = ", mReturnDestination);
            sb.AppendFormat("{0}.{1}(", mCalledMethod.Container, mCalledMethod);
            for (int index = 0; index < mParameterSources.Count; ++index)
            {
                if (index > 0) sb.Append(", ");
                sb.AppendFormat("{0}", mParameterSources[index]);
            }
            sb.AppendFormat(")");
            return sb.ToString();
        }

        internal override void Transform(LLFunction pFunction)
        {
            List<LLLocation> parameters = new List<LLLocation>();
            mParameterSources.ForEach(l => parameters.Add(l.Load(pFunction)));

            for (int index = 0; index < parameters.Count; ++index)
                parameters[index] = pFunction.CurrentBlock.EmitConversion(parameters[index], mCalledMethod.LLFunction.Parameters[index].Type);

            LLLocation locationReturn = null;
            if (mReturnDestination != null)
                locationReturn = LLTemporaryLocation.Create(pFunction.CreateTemporary(mReturnDestination.Type.LLType));
            pFunction.CurrentBlock.EmitCall(locationReturn, LLFunctionLocation.Create(mCalledMethod.LLFunction), parameters);
            if (mReturnDestination != null) mReturnDestination.Store(pFunction, locationReturn);
        }
    }
}
