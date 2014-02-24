using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public sealed class HLMultiplyInstruction : HLInstruction
    {
        public static HLMultiplyInstruction Create(HLMethod pMethod, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource)
        {
            HLMultiplyInstruction instruction = new HLMultiplyInstruction(pMethod);
            instruction.mDestination = pDestination;
            instruction.mLeftOperandSource = pLeftOperandSource;
            instruction.mRightOperandSource = pRightOperandSource;
            return instruction;
        }

        private HLLocation mDestination = null;
        private HLLocation mLeftOperandSource = null;
        private HLLocation mRightOperandSource = null;

        private HLMultiplyInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString() { return string.Format("{0} = {1} * {2}", mDestination, mLeftOperandSource, mRightOperandSource); }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationLeftOperand = mLeftOperandSource.Load(pFunction);
            LLLocation locationRightOperand = mRightOperandSource.Load(pFunction);

            LLType typeOperands = LLModule.BinaryResult(locationLeftOperand.Type, locationRightOperand.Type);
            locationLeftOperand = pFunction.CurrentBlock.EmitConversion(locationLeftOperand, typeOperands);
            locationRightOperand = pFunction.CurrentBlock.EmitConversion(locationRightOperand, typeOperands);

            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(typeOperands));
            pFunction.CurrentBlock.EmitMultiply(locationTemporary, locationLeftOperand, locationRightOperand);

            mDestination.Store(pFunction, locationTemporary);
        }
    }
}
