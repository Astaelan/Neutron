using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLFunctionLocation : LLLocation
    {
        public static LLFunctionLocation Create(LLFunction pFunction)
        {
            LLFunctionLocation location = new LLFunctionLocation(LLModule.GetOrCreateFunctionType(pFunction.ReturnType, pFunction.Parameters.ToTypeList()).PointerDepthPlusOne);
            location.mFunction = pFunction;
            return location;
        }

        private LLFunction mFunction = null;

        private LLFunctionLocation(LLType pType) : base(pType) { }

        public LLFunction Function { get { return mFunction; } }

        public override string ToString() { return mFunction.ToString(); }

        public override LLLocation Load(LLInstructionBlock pBlock) { return this; }
    }
}
