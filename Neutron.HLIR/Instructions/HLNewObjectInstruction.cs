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
    }
}
