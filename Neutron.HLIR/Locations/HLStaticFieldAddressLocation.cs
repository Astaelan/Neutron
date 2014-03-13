using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Instructions;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLStaticFieldAddressLocation : HLLocation
    {
        public static HLStaticFieldAddressLocation Create(HLField pStaticField)
        {
            HLStaticFieldAddressLocation location = new HLStaticFieldAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pStaticField.Type.Definition, HLDomain.Host.InternFactory, pStaticField.Type.Definition)));
            location.mStaticField = pStaticField;
            return location;
        }

        private HLStaticFieldAddressLocation(HLType pType) : base(pType) { }

        private HLField mStaticField = null;
        public HLField StaticField { get { return mStaticField; } }

        public override string ToString() { return string.Format("({0})&{1}.{2}", Type, mStaticField.Container, mStaticField); }

        internal static void CheckStaticConstructorCalled(LLFunction pFunction, HLType pType)
        {
            if (pType.StaticConstructor == null) return;
            if (pType.StaticConstructor.LLFunction == pFunction) return;
            LLLocation locationConstructorCalled = LLGlobalLocation.Create(LLModule.GetGlobal(pType.ToString()));
            LLLocation locationConstructorCalledOriginal = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationConstructorCalled.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitCompareExchange(locationConstructorCalledOriginal, locationConstructorCalled, LLLiteralLocation.Create(LLLiteral.Create(locationConstructorCalledOriginal.Type, "0")), LLLiteralLocation.Create(LLLiteral.Create(locationConstructorCalledOriginal.Type, "1")), LLCompareExchangeOrdering.acq_rel);

            LLLocation locationConstructorCall = LLTemporaryLocation.Create(pFunction.CreateTemporary(LLModule.BooleanType));
            pFunction.CurrentBlock.EmitCompareIntegers(locationConstructorCall, locationConstructorCalledOriginal, LLLiteralLocation.Create(LLLiteral.Create(locationConstructorCalledOriginal.Type, "0")), LLCompareIntegersCondition.eq);

            LLLabel labelTrue = pFunction.CreateLabel(pFunction.Labels.Count);
            LLLabel labelFalse = pFunction.CreateLabel(pFunction.Labels.Count);
            LLLabel labelNext = pFunction.CreateLabel(pFunction.Labels.Count);
            pFunction.CurrentBlock.EmitBranch(locationConstructorCall, labelTrue, labelFalse);

            LLInstructionBlock blockTrue = pFunction.CreateBlock(labelTrue);
            List<LLLocation> parameters = new List<LLLocation>();
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(pType.StaticConstructor.LLFunction.Parameters[0].Type, "zeroinitializer")));
            blockTrue.EmitCall(null, LLFunctionLocation.Create(pType.StaticConstructor.LLFunction), parameters);
            blockTrue.EmitGoto(labelNext);

            LLInstructionBlock blockFalse = pFunction.CreateBlock(labelFalse);
            blockFalse.EmitGoto(labelNext);

            pFunction.CurrentBlock = pFunction.CreateBlock(labelNext);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            CheckStaticConstructorCalled(pFunction, StaticField.Container);
            return LLGlobalLocation.Create(LLModule.GetGlobal(StaticField.Container.ToString() + "." + StaticField.ToString()));
        }
    }
}
