using Neutron.LLIR;
using Neutron.LLIR.Instructions;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Instructions
{
    public enum HLCompareType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public sealed class HLCompareInstruction : HLInstruction
    {
        public static HLCompareInstruction Create(HLMethod pMethod, HLCompareType pCompareType, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource)
        {
            HLCompareInstruction instruction = new HLCompareInstruction(pMethod);
            instruction.mCompareType = pCompareType;
            instruction.mDestination = pDestination;
            instruction.mLeftOperandSource = pLeftOperandSource;
            instruction.mRightOperandSource = pRightOperandSource;
            return instruction;
        }

        private HLCompareType mCompareType = HLCompareType.Equal;
        private HLLocation mDestination = null;
        private HLLocation mLeftOperandSource = null;
        private HLLocation mRightOperandSource = null;

        private HLCompareInstruction(HLMethod pMethod) : base(pMethod) { }

        public override string ToString()
        {
            string operation = "";
            switch (mCompareType)
            {
                case HLCompareType.Equal: operation = "=="; break;
                case HLCompareType.NotEqual: operation = "!="; break;
                case HLCompareType.GreaterThan: operation = ">"; break;
                case HLCompareType.GreaterThanOrEqual: operation = ">="; break;
                case HLCompareType.LessThan: operation = "<"; break;
                case HLCompareType.LessThanOrEqual: operation = "<="; break;
                default: throw new NotSupportedException();
            }
            return string.Format("{0} = {1} {2} {3}", mDestination, mLeftOperandSource, operation, mRightOperandSource);
        }

        internal override void Transform(LLFunction pFunction)
        {
            LLLocation locationLeftOperand = mLeftOperandSource.Load(pFunction);
            LLLocation locationRightOperand = mRightOperandSource.Load(pFunction);

            LLType typeOperands = LLModule.BinaryResult(locationLeftOperand.Type, locationRightOperand.Type);
            locationLeftOperand = pFunction.CurrentBlock.EmitConversion(locationLeftOperand, typeOperands);
            locationRightOperand = pFunction.CurrentBlock.EmitConversion(locationRightOperand, typeOperands);

            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(LLModule.BooleanType));

            if (typeOperands.Primitive == LLPrimitive.Float)
            {
                LLCompareFloatsCondition condition = LLCompareFloatsCondition.oeq;
                switch (mCompareType)
                {
                    case HLCompareType.Equal: condition = LLCompareFloatsCondition.oeq; break;
                    case HLCompareType.NotEqual: condition = LLCompareFloatsCondition.one; break;
                    case HLCompareType.GreaterThan: condition = LLCompareFloatsCondition.ogt; break;
                    case HLCompareType.GreaterThanOrEqual: condition = LLCompareFloatsCondition.oge; break;
                    case HLCompareType.LessThan: condition = LLCompareFloatsCondition.olt; break;
                    case HLCompareType.LessThanOrEqual: condition = LLCompareFloatsCondition.ole; break;
                    default: throw new NotSupportedException();
                }
                pFunction.CurrentBlock.EmitCompareFloats(locationTemporary, locationLeftOperand, locationRightOperand, condition);
            }
            else
            {
                LLCompareIntegersCondition condition = LLCompareIntegersCondition.eq;
                switch (mCompareType)
                {
                    case HLCompareType.Equal: condition = LLCompareIntegersCondition.eq; break;
                    case HLCompareType.NotEqual: condition = LLCompareIntegersCondition.ne; break;
                    case HLCompareType.GreaterThan: condition = typeOperands.Primitive == LLPrimitive.Unsigned ? LLCompareIntegersCondition.ugt : LLCompareIntegersCondition.sgt; break;
                    case HLCompareType.GreaterThanOrEqual: condition = typeOperands.Primitive == LLPrimitive.Unsigned ? LLCompareIntegersCondition.uge : LLCompareIntegersCondition.sge; break;
                    case HLCompareType.LessThan: condition = typeOperands.Primitive == LLPrimitive.Unsigned ? LLCompareIntegersCondition.ult : LLCompareIntegersCondition.slt; break;
                    case HLCompareType.LessThanOrEqual: condition = typeOperands.Primitive == LLPrimitive.Unsigned ? LLCompareIntegersCondition.ule : LLCompareIntegersCondition.sle; break;
                    default: throw new NotSupportedException();
                }
                pFunction.CurrentBlock.EmitCompareIntegers(locationTemporary, locationLeftOperand, locationRightOperand, condition);
            }

            mDestination.Store(pFunction, locationTemporary);
        }
    }
}
