using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLNewArrayInstruction : HLInstruction
    {
        public static HLNewArrayInstruction Create(HLMethod pMethod, HLLocation pDestinationSource, HLLocation pSizeSource, HLType pElementType)
        {
            HLNewArrayInstruction instruction = new HLNewArrayInstruction(pMethod);
            instruction.mDestinationSource = pDestinationSource;
            instruction.mSizeSource = pSizeSource;
            instruction.mElementType = pElementType;
            return instruction;
        }

        private HLLocation mDestinationSource = null;
        private HLLocation mSizeSource = null;
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
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRootFunction.Parameters[1].Type, "null")));
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRootFunction), parameters);

            LLLocation locationSize = mSizeSource.Load(pFunction);
            LLLocation locationElementsSize = locationSize;
            if (mElementType.VariableSize > 1)
            {
                locationElementsSize = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationSize.Type));
                pFunction.CurrentBlock.EmitMultiply(locationElementsSize, locationSize, LLLiteralLocation.Create(LLLiteral.Create(locationSize.Type, mElementType.VariableSize.ToString())));
            }
            LLLocation locationTotalSize = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationSize.Type));
            pFunction.CurrentBlock.EmitAdd(locationTotalSize, locationElementsSize, LLLiteralLocation.Create(LLLiteral.Create(locationSize.Type, (HLDomain.SizeOfPointer + 4).ToString())));
            locationTotalSize = pFunction.CurrentBlock.EmitConversion(locationTotalSize, HLDomain.GCAllocFunction.Parameters[1].Type);

            parameters.Clear();
            parameters.Add(locationArrayReference);
            parameters.Add(locationTotalSize);
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCAllocFunction), parameters);

            LLLocation locationArrayPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayReference.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationArrayPointer, locationArrayReference);
            locationArrayPointer = pFunction.CurrentBlock.EmitConversion(locationArrayPointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1));
            
            LLLocation locationArraySizePointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
            pFunction.CurrentBlock.EmitGetElementPointer(locationArraySizePointer, locationArrayPointer, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), HLDomain.SizeOfPointer.ToString())));
            locationArraySizePointer = pFunction.CurrentBlock.EmitConversion(locationArraySizePointer, locationSize.Type.PointerDepthPlusOne);
            pFunction.CurrentBlock.EmitStore(locationArraySizePointer, locationSize);
        }
    }
}
