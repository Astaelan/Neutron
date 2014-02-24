using Neutron.LLIR;
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
            mDestination.Store(pFunction, locationSource);
        }
    }
}
