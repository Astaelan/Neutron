using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLAssignmentInstruction : HLInstruction
    {
        public static HLAssignmentInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pSource)
        {
            HLAssignmentInstruction instruction = new HLAssignmentInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mSource = pSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mSource = null;

        private HLAssignmentInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = {1}", mDestination, mSource); }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationSource = mSource.Load(pFunction);
            if (mSource.Type.Definition.IsValueType && mDestination.Type == HLDomain.SystemObject)
            {
                // Boxing
                List<LLLocation> parameters = new List<LLLocation>();
                LLLocation locationObjectReference = LLTemporaryLocation.Create(pFunction.CreateTemporary(HLDomain.SystemObject.LLType.PointerDepthPlusOne));
                pFunction.CurrentBlock.EmitAllocate(locationObjectReference);
                pFunction.CurrentBlock.EmitStore(locationObjectReference, LLLiteralLocation.Create(LLLiteral.Create(locationObjectReference.Type.PointerDepthMinusOne, "zeroinitializer")));

                parameters.Add(locationObjectReference);
                parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRoot.Parameters[1].Type, "null")));
                pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRoot), parameters);

                LLLocation locationTotalSize = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[1].Type, (HLDomain.SizeOfPointer + mSource.Type.CalculatedSize).ToString()));
                LLLocation locationHandle = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[2].Type, mSource.Type.RuntimeTypeHandle.ToString()));

                parameters.Clear();
                parameters.Add(locationObjectReference);
                parameters.Add(locationTotalSize);
                parameters.Add(locationHandle);
                pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCAllocate), parameters);

                LLLocation locationObject = LLTemporaryLocation.Create(pFunction.CreateTemporary(HLDomain.SystemObject.LLType));
                pFunction.CurrentBlock.EmitLoad(locationObject, locationObjectReference);

                LLLocation locationObjectValue = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationObject.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationObjectValue, locationObject, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), HLDomain.SizeOfPointer.ToString())));
                locationObjectValue = pFunction.CurrentBlock.EmitConversion(locationObjectValue, mSource.Type.LLType.PointerDepthPlusOne);

                pFunction.CurrentBlock.EmitStore(locationObjectValue, locationSource);
                locationSource = locationObject;
            }
            else if (mSource.Type == HLDomain.SystemObject && mDestination.Type.Definition.IsValueType)
            {
                // Unboxing

                // TODO: Branch on destination type handle not matching object handle, and throw exception
                LLLocation locationObjectValue = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationSource.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationObjectValue, locationSource, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), HLDomain.SizeOfPointer.ToString())));
                locationObjectValue = pFunction.CurrentBlock.EmitConversion(locationObjectValue, mDestination.Type.LLType.PointerDepthPlusOne);

                LLLocation locationValue = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationObjectValue.Type.PointerDepthMinusOne));
                pFunction.CurrentBlock.EmitLoad(locationValue, locationObjectValue);
                locationSource = locationValue;
            }
            mDestination.Store(pFunction, locationSource);
        }
    }
}
