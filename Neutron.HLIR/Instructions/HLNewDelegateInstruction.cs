using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLNewDelegateInstruction : HLInstruction
    {
        public static HLNewDelegateInstruction Create(HLMethod pMethod, HLType pNewDelegateType, HLLocation pDestinationSource, HLLocation pInstanceSource, HLMethod pMethodCalled, bool pVirtual)
        {
            HLNewDelegateInstruction instruction = new HLNewDelegateInstruction(pMethod);
            instruction.mNewDelegateType = pNewDelegateType;
            instruction.mDestinationSource = pDestinationSource;
            instruction.mInstanceSource = pInstanceSource;
            instruction.mMethodCalled = pMethodCalled;
            instruction.mVirtual = pVirtual;
            return instruction;
        }

        private HLType mNewDelegateType = null;
        private HLLocation mDestinationSource = null;
        private HLLocation mInstanceSource = null;
        private HLMethod mMethodCalled = null;
        private bool mVirtual = false;

        private HLNewDelegateInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            return string.Format("newdelegate({0}, {1}, {2}, {3}, {4})", mDestinationSource, mNewDelegateType, mInstanceSource == null ? "null" : mInstanceSource.ToString(), mMethodCalled, mVirtual);
        }

        internal override void Transform(LLFunction pFunction)
        {
            List<LLLocation> parameters = new List<LLLocation>();
            LLLocation locationDelegateReference = mDestinationSource.Load(pFunction);
            pFunction.CurrentBlock.EmitStore(locationDelegateReference, LLLiteralLocation.Create(LLLiteral.Create(locationDelegateReference.Type.PointerDepthMinusOne, "zeroinitializer")));

            parameters.Add(locationDelegateReference);
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRoot.Parameters[1].Type, "null")));
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRoot), parameters);

            LLLocation locationTotalSize = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[1].Type, mNewDelegateType.CalculatedSize.ToString()));

            LLLocation locationHandle = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[2].Type, mNewDelegateType.RuntimeTypeHandle.ToString()));

            parameters.Clear();
            parameters.Add(locationDelegateReference);
            parameters.Add(locationTotalSize);
            parameters.Add(locationHandle);
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCAllocate), parameters);

            LLLocation locationDelegate = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationDelegateReference.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationDelegate, locationDelegateReference);

            LLLocation locationTargetObj = null;
            if (mInstanceSource != null)
            {
                LLLocation locationTargetObjPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationDelegate.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationTargetObjPointer, locationDelegate, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), mNewDelegateType.Fields["mTargetObj"].Offset.ToString())));
                locationTargetObjPointer = pFunction.CurrentBlock.EmitConversion(locationTargetObjPointer, mInstanceSource.Type.LLType.PointerDepthPlusOne);
                locationTargetObj = mInstanceSource.Load(pFunction);
                pFunction.CurrentBlock.EmitStore(locationTargetObjPointer, locationTargetObj);
            }

            LLLocation locationTargetMethodPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationDelegate.Type));
            pFunction.CurrentBlock.EmitGetElementPointer(locationTargetMethodPointer, locationDelegate, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), mNewDelegateType.Fields["mTargetMethod"].Offset.ToString())));
            locationTargetMethodPointer = pFunction.CurrentBlock.EmitConversion(locationTargetMethodPointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateSignedType(32), 1));
            LLLocation locationTargetMethod = null;
            if (!mVirtual)
                locationTargetMethod = LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), mMethodCalled.RuntimeMethodHandle.ToString()));
            else
            {
                LLType typeFunction = LLModule.GetOrCreateFunctionType(mMethodCalled.ReturnType == null ? null : mMethodCalled.ReturnType.LLType, mMethodCalled.Parameters.ConvertAll(p => p.Type.LLType));
                int virtualIndex = mMethodCalled.Container.VirtualLookup[mMethodCalled];

                parameters.Clear();
                parameters.Add(locationTargetObj);
                parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), virtualIndex.ToString())));
                for (int index = 0; index < parameters.Count; ++index)
                    parameters[index] = pFunction.CurrentBlock.EmitConversion(parameters[index], HLDomain.VTableHandleLookup.Parameters[index].Type);
                locationTargetMethod = LLTemporaryLocation.Create(pFunction.CreateTemporary(HLDomain.VTableHandleLookup.ReturnType));
                pFunction.CurrentBlock.EmitCall(locationTargetMethod, LLFunctionLocation.Create(HLDomain.VTableHandleLookup), parameters);
                locationTargetMethod = pFunction.CurrentBlock.EmitConversion(locationTargetMethod, LLModule.GetOrCreateSignedType(32));
            }
            pFunction.CurrentBlock.EmitStore(locationTargetMethodPointer, locationTargetMethod);
        }
    }
}
