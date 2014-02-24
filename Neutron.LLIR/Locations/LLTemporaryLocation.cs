using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLTemporaryLocation : LLLocation
    {
        public static LLTemporaryLocation Create(LLTemporary pTemporary)
        {
            LLTemporaryLocation location = new LLTemporaryLocation(pTemporary.Type);
            location.mTemporary = pTemporary;
            return location;
        }

        private LLTemporary mTemporary = null;

        private LLTemporaryLocation(LLType pType) : base(pType) { }

        public LLTemporary Temporary { get { return mTemporary; } }

        public override string ToString() { return mTemporary.ToString(); }

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
