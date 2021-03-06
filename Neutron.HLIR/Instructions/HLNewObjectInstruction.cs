﻿using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLNewObjectInstruction : HLInstruction
    {
        public static HLNewObjectInstruction Create(HLMethod pMethod, HLType pNewObjectType, HLLocation pDestinationSource)
        {
            HLNewObjectInstruction instruction = new HLNewObjectInstruction(pMethod);
            instruction.mNewObjectType = pNewObjectType;
            instruction.mDestinationSource = pDestinationSource;
            return instruction;
        }

        private HLType mNewObjectType = null;
        private HLLocation mDestinationSource = null;

        private HLNewObjectInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            return string.Format("newobject({0}, {1})", mDestinationSource, mNewObjectType);
        }

        internal override void Transform(LLFunction pFunction)
        {
            List<LLLocation> parameters = new List<LLLocation>();
            LLLocation locationObjectReference = mDestinationSource.Load(pFunction);
            pFunction.CurrentBlock.EmitStore(locationObjectReference, LLLiteralLocation.Create(LLLiteral.Create(locationObjectReference.Type.PointerDepthMinusOne, "zeroinitializer")));

            parameters.Add(locationObjectReference);
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRoot.Parameters[1].Type, "null")));
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRoot), parameters);

            LLLocation locationTotalSize = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[1].Type, mNewObjectType.CalculatedSize.ToString()));

            LLLocation locationHandle = LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCAllocate.Parameters[2].Type, mNewObjectType.RuntimeTypeHandle.ToString()));

            parameters.Clear();
            parameters.Add(locationObjectReference);
            parameters.Add(locationTotalSize);
            parameters.Add(locationHandle);
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCAllocate), parameters);
        }
    }
}
