using Neutron.HLIR.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLInstructionBlock
    {
        public static HLInstructionBlock Create(HLMethod pMethod, HLLabel pStartLabel)
        {
            HLInstructionBlock block = new HLInstructionBlock();
            block.mMethod = pMethod;
            block.mStartLabel = pStartLabel;
            block.EmitLabel(pStartLabel);
            return block;
        }

        private HLMethod mMethod = null;
        private HLLabel mStartLabel = null;
        private List<HLInstruction> mInstructions = new List<HLInstruction>();
        private bool mTerminated = false;

        private HLInstructionBlock() { }

        public HLMethod Method { get { return mMethod; } }

        public HLLabel StartLabel { get { return mStartLabel; } }

        public List<HLInstruction> Instructions { get { return mInstructions; } }

        public bool Terminated { get { return mTerminated; } }

        public void Terminate(HLLabel pNextBlockStartLabel)
        {
            if (!mTerminated)
            {
                if (pNextBlockStartLabel == null) EmitReturn(null);
                else EmitGoto(pNextBlockStartLabel);
            }
        }

        private void Emit(HLInstruction pInstruction)
        {
            if (mInstructions == null) mInstructions = new List<HLInstruction>();
            mInstructions.Add(pInstruction);
            if (pInstruction.AffectsTermination) mTerminated = pInstruction.IsTerminator;
        }
        public void EmitAdd(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLAddInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitAssignment(HLLocation pDestination, HLLocation pSource) { Emit(HLAssignmentInstruction.Create(mMethod, pDestination, pSource)); }
        public void EmitBitwiseAnd(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLBitwiseAndInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitBitwiseNegate(HLLocation pDestination, HLLocation pSource) { Emit(HLBitwiseNegateInstruction.Create(mMethod, pDestination, pSource)); }
        public void EmitBitwiseNot(HLLocation pDestination, HLLocation pSource) { Emit(HLBitwiseNotInstruction.Create(mMethod, pDestination, pSource)); }
        public void EmitBitwiseOr(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLBitwiseOrInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitBitwiseXor(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLBitwiseXorInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitBranch(HLLocation pConditionSource, HLLabel pTrueLabel, HLLabel pFalseLabel) { Emit(HLBranchInstruction.Create(mMethod, pConditionSource, pTrueLabel, pFalseLabel)); }
        public void EmitCall(HLMethod pCalledMethod, bool pVirtual, HLLocation pReturnDestination, List<HLLocation> pParameterSources) { Emit(HLCallInstruction.Create(mMethod, pCalledMethod, pVirtual, pReturnDestination, pParameterSources)); }
        public void EmitCompare(HLCompareType pCompareType, HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLCompareInstruction.Create(mMethod, pCompareType, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitDivide(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLDivideInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitGoto(HLLabel pTargetLabel) { Emit(HLGotoInstruction.Create(mMethod, pTargetLabel)); }
        public void EmitLabel(HLLabel pLabel) { Emit(HLLabelInstruction.Create(mMethod, pLabel)); }
        public void EmitModulus(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLModulusInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitMultiply(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLMultiplyInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitNewArray(HLLocation pDestinationSource, HLLocation pSizeSource, HLType pArrayType, HLType pElementType) { Emit(HLNewArrayInstruction.Create(mMethod, pDestinationSource, pSizeSource, pArrayType, pElementType)); }
        public void EmitNewDelegate(HLType pNewDelegateType, HLLocation pDestinationSource, HLLocation pInstanceSource, HLMethod pMethodCalled, bool pVirtual) { Emit(HLNewDelegateInstruction.Create(mMethod, pNewDelegateType, pDestinationSource, pInstanceSource, pMethodCalled, pVirtual)); }
        public void EmitNewObject(HLType pNewObjectType, HLLocation pDestinationSource) { Emit(HLNewObjectInstruction.Create(mMethod, pNewObjectType, pDestinationSource)); }
        public void EmitReturn(HLLocation pSource) { Emit(HLReturnInstruction.Create(mMethod, pSource)); }
        public void EmitSubtract(HLLocation pDestination, HLLocation pLeftOperandSource, HLLocation pRightOperandSource) { Emit(HLSubtractInstruction.Create(mMethod, pDestination, pLeftOperandSource, pRightOperandSource)); }
        public void EmitSwitch(HLLocation pConditionSource, HLLabel pDefaultLabel, List<Tuple<HLLiteralLocation, HLLabel>> pCases) { Emit(HLSwitchInstruction.Create(mMethod, pConditionSource, pDefaultLabel, pCases)); }
    }
}
