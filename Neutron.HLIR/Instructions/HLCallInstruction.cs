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
        public static HLCallInstruction Create(HLMethod pMethod, HLMethod pCalledMethod, bool pVirtual, HLLocation pReturnDestination, List<HLLocation> pParameterSources)
        {
            HLCallInstruction instruction = new HLCallInstruction(pMethod);
            instruction.mCalledMethod = pCalledMethod;
            instruction.mVirtual = pVirtual;
            instruction.mReturnDestination = pReturnDestination;
            instruction.mParameterSources = new List<HLLocation>(pParameterSources);
            return instruction;
        }

        private HLMethod mCalledMethod = null;
        private bool mVirtual = false;
        private HLLocation mReturnDestination = null;
        private List<HLLocation> mParameterSources = null;

        private HLCallInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (mReturnDestination != null) sb.AppendFormat("{0} = ", mReturnDestination);
            if (mVirtual) sb.Append("virtual ");
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
            LLLocation locationReturn = null;
            LLLocation locationFunction = null;
            if (!mVirtual) locationFunction = LLFunctionLocation.Create(mCalledMethod.LLFunction);
            else
            {
                // TODO: Lookup correct function pointer from the runtime vtable
                // and assign to locationFunction
                LLType typeFunction = LLModule.GetOrCreateFunctionType(mCalledMethod.ReturnType == null ? null : mCalledMethod.ReturnType.LLType, mParameterSources.ConvertAll(l => l.Type.LLType));
                int virtualIndex = mCalledMethod.Container.VirtualLookup[mCalledMethod];
                parameters.Add(mParameterSources[0].Load(pFunction));
                parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), virtualIndex.ToString())));
                for (int index = 0; index < parameters.Count; ++index)
                    parameters[index] = pFunction.CurrentBlock.EmitConversion(parameters[index], HLDomain.VTableLookupFunction.Parameters[index].Type);
                locationReturn = LLTemporaryLocation.Create(pFunction.CreateTemporary(HLDomain.VTableLookupFunction.ReturnType));
                pFunction.CurrentBlock.EmitCall(locationReturn, LLFunctionLocation.Create(HLDomain.VTableLookupFunction), parameters);
                locationFunction = pFunction.CurrentBlock.EmitConversion(locationReturn, typeFunction.PointerDepthPlusOne);
            }

            parameters.Clear();
            mParameterSources.ForEach(l => parameters.Add(l.Load(pFunction)));

            for (int index = 0; index < parameters.Count; ++index)
                parameters[index] = pFunction.CurrentBlock.EmitConversion(parameters[index], mCalledMethod.LLFunction.Parameters[index].Type);

            locationReturn = null;
            if (mReturnDestination != null)
                locationReturn = LLTemporaryLocation.Create(pFunction.CreateTemporary(mReturnDestination.Type.LLType));

            pFunction.CurrentBlock.EmitCall(locationReturn, locationFunction, parameters);
            if (mReturnDestination != null) mReturnDestination.Store(pFunction, locationReturn);
        }
    }
}
