using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLGlobalLocation : LLLocation
    {
        public static LLGlobalLocation Create(LLGlobal pGlobal)
        {
            LLGlobalLocation location = new LLGlobalLocation(pGlobal.Type);
            location.mGlobal = pGlobal;
            return location;
        }

        private LLGlobal mGlobal = null;

        private LLGlobalLocation(LLType pType) : base(pType) { }

        public LLGlobal Global { get { return mGlobal; } }

        public override string ToString() { return mGlobal.ToString(); }

        public override LLLocation Load(LLInstructionBlock pBlock)
        {
            LLLocation destination = LLTemporaryLocation.Create(pBlock.Function.CreateTemporary(Type.PointerDepthMinusOne));
            pBlock.EmitLoad(destination, this);
            return destination;
        }

        public override void Store(LLInstructionBlock pBlock, LLLocation pSource)
        {
            LLLocation source = pBlock.EmitConversion(pSource, Type.PointerDepthMinusOne);
            pBlock.EmitStore(this, source);
        }
    }
}
