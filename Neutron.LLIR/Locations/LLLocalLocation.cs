using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLLocalLocation : LLLocation
    {
        public static LLLocalLocation Create(LLLocal pLocal)
        {
            LLLocalLocation location = new LLLocalLocation(pLocal.Type);
            location.mLocal = pLocal;
            return location;
        }

        private LLLocal mLocal = null;

        private LLLocalLocation(LLType pType) : base(pType) { }

        public LLLocal Local { get { return mLocal; } }

        public override string ToString() { return mLocal.ToString(); }

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
