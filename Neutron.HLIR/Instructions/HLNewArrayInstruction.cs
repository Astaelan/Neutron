﻿using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLNewArrayInstruction : HLInstruction
    {
        public static HLNewArrayInstruction Create(HLMethod pMethod, HLLocation pDestinationSource, HLLocation pSizeSource, HLType pArrayType, HLType pElementType)
        {
            HLNewArrayInstruction instruction = new HLNewArrayInstruction(pMethod);
            instruction.mDestinationSource = pDestinationSource;
            instruction.mSizeSource = pSizeSource;
            instruction.mArrayType = pArrayType;
            instruction.mElementType = pElementType;
            return instruction;
        }

        private HLLocation mDestinationSource = null;
        private HLLocation mSizeSource = null;
        private HLType mArrayType = null;
        private HLType mElementType = null;

        private HLNewArrayInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            return string.Format("newarray({0}, {1})", mDestinationSource, mSizeSource);
        }

        internal override void Transform(LLFunction pFunction)
        {
            List<LLLocation> parameters = new List<LLLocation>();
            LLLocation locationArrayReference = mDestinationSource.Load(pFunction);
            pFunction.CurrentBlock.EmitStore(locationArrayReference, LLLiteralLocation.Create(LLLiteral.Create(locationArrayReference.Type.PointerDepthMinusOne, "zeroinitializer")));

            parameters.Add(locationArrayReference);
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRoot.Parameters[1].Type, "null")));
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRoot), parameters);

            LLLocation locationSize = mSizeSource.Load(pFunction);
            LLLocation locationElementsSize = locationSize;
            if (mElementType.VariableSize > 1)
            {
                locationElementsSize = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationSize.Type));
                pFunction.CurrentBlock.EmitMultiply(locationElementsSize, locationSize, LLLiteralLocation.Create(LLLiteral.Create(locationSize.Type, mElementType.VariableSize.ToString())));
            }
            LLLocation locationTotalSize = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationSize.Type));
            pFunction.CurrentBlock.EmitAdd(locationTotalSize, locationElementsSize, LLLiteralLocation.Create(LLLiteral.Create(locationSize.Type, (HLDomain.SystemArray.CalculatedSize).ToString())));
            locationTotalSize = pFunction.CurrentBlock.EmitConversion(locationTotalSize, HLDomain.GCAllocate.Parameters[1].Type);

            LLLocation locationHandle = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[2].Type, mArrayType.RuntimeTypeHandle.ToString()));

            parameters.Clear();
            parameters.Add(locationArrayReference);
            parameters.Add(locationTotalSize);
            parameters.Add(locationHandle);
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCAllocate), parameters);

            LLLocation locationArrayPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayReference.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationArrayPointer, locationArrayReference);
            locationArrayPointer = pFunction.CurrentBlock.EmitConversion(locationArrayPointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1));
            
            LLLocation locationArraySizePointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
            pFunction.CurrentBlock.EmitGetElementPointer(locationArraySizePointer, locationArrayPointer, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), HLDomain.SystemObject.CalculatedSize.ToString())));
            locationArraySizePointer = pFunction.CurrentBlock.EmitConversion(locationArraySizePointer, locationSize.Type.PointerDepthPlusOne);
            pFunction.CurrentBlock.EmitStore(locationArraySizePointer, locationSize);
        }
    }
}
