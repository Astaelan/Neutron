using Neutron.LLIR.Instructions;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLInstructionBlock
    {
        public static LLInstructionBlock Create(LLFunction pFunction, LLLabel pStartLabel)
        {
            LLInstructionBlock block = new LLInstructionBlock();
            block.mFunction = pFunction;
            block.mStartLabel = pStartLabel;
            block.EmitLabel(pStartLabel);
            return block;
        }

        private LLFunction mFunction = null;
        private LLLabel mStartLabel = null;
        private List<LLInstruction> mInstructions = new List<LLInstruction>();
        private bool mTerminated = false;

        private LLInstructionBlock() { }

        public LLFunction Function { get { return mFunction; } }

        public LLLabel StartLabel { get { return mStartLabel; } }

        public List<LLInstruction> Instruction { get { return mInstructions; } }

        public void Terminate(LLLabel pNextBlockStartLabel)
        {
            if (!mTerminated)
            {
                if (pNextBlockStartLabel == null) EmitReturn(null);
                else EmitGoto(pNextBlockStartLabel);
            }
        }

        private void Emit(LLInstruction pInstruction)
        {
            if (mInstructions == null) mInstructions = new List<LLInstruction>();
            mInstructions.Add(pInstruction);
            if (pInstruction.AffectsTermination) mTerminated = pInstruction.IsTerminator;
        }
        public void EmitAdd(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLAddInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitAnd(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLAndInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitAllocate(LLLocation pDestination) { Emit(LLAllocateInstruction.Create(mFunction, pDestination)); }
        public void EmitBitcast(LLLocation pDestination, LLLocation pSource) { Emit(LLBitcastInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitBranch(LLLocation pConditionSource, LLLabel pTrueTargetLabel, LLLabel pFalseTargetLabel) { Emit(LLBranchInstruction.Create(mFunction, pConditionSource, pTrueTargetLabel, pFalseTargetLabel)); }
        public void EmitCall(LLLocation pReturnDestination, LLLocation pFunctionSource, List<LLLocation> pParameterSources) { Emit(LLCallInstruction.Create(mFunction, pReturnDestination, pFunctionSource, pParameterSources)); }
        public void EmitComment(string pComment) { Emit(LLCommentInstruction.Create(mFunction, pComment)); }
        public void EmitCompareExchange(LLLocation pDestination, LLLocation pPointerSource, LLLocation pComparedSource, LLLocation pNewSource, LLCompareExchangeOrdering pOrdering) { Emit(LLCompareExchangeInstruction.Create(mFunction, pDestination, pPointerSource, pComparedSource, pNewSource, pOrdering)); }
        public void EmitCompareFloats(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource, LLCompareFloatsCondition pCondition) { Emit(LLCompareFloatsInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource, pCondition)); }
        public void EmitCompareIntegers(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource, LLCompareIntegersCondition pCondition) { Emit(LLCompareIntegersInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource, pCondition)); }
        public void EmitDivide(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLDivideInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitExtend(LLLocation pDestination, LLLocation pSource) { Emit(LLExtendInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitFloatToInteger(LLLocation pDestination, LLLocation pSource) { Emit(LLFloatToIntegerInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitGetElementPointer(LLLocation pDestination, LLLocation pPointerSource, params LLLocation[] pIndexSources) { Emit(LLGetElementPointerInstruction.Create(mFunction, pDestination, pPointerSource, pIndexSources)); }
        public void EmitGoto(LLLabel pTargetLabel) { Emit(LLGotoInstruction.Create(mFunction, pTargetLabel)); }
        public void EmitIntegerToFloat(LLLocation pDestination, LLLocation pSource) { Emit(LLIntegerToFloatInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitIntegerToPointer(LLLocation pDestination, LLLocation pSource) { Emit(LLIntegerToPointerInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitLabel(LLLabel pLabel) { Emit(LLLabelInstruction.Create(mFunction, pLabel)); }
        public void EmitLeftShift(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLLeftShiftInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitLoad(LLLocation pDestination, LLLocation pSource) { Emit(LLLoadInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitMultiply(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLMultiplyInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitOr(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLOrInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitPointerToInteger(LLLocation pDestination, LLLocation pSource) { Emit(LLPointerToIntegerInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitReturn(LLLocation pSource) { Emit(LLReturnInstruction.Create(mFunction, pSource)); }
        public void EmitSignedRightShift(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLSignedRightShiftInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitStore(LLLocation pDestination, LLLocation pSource) { Emit(LLStoreInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitSubtract(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLSubtractInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitSwitch(LLLocation pConditionSource, LLLabel pDefaultTargetLabel, List<LLSwitchCase> pCases) { Emit(LLSwitchInstruction.Create(mFunction, pConditionSource, pDefaultTargetLabel, pCases)); }
        public void EmitTruncate(LLLocation pDestination, LLLocation pSource) { Emit(LLTruncateInstruction.Create(mFunction, pDestination, pSource)); }
        public void EmitUnsignedRightShift(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLUnsignedRightShiftInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }
        public void EmitXor(LLLocation pDestination, LLLocation pLeftSource, LLLocation pRightSource) { Emit(LLXorInstruction.Create(mFunction, pDestination, pLeftSource, pRightSource)); }

        public LLLocation EmitConversion(LLLocation pSource, LLType pType)
        {
            if (pSource.Type == pType) return pSource;
            LLLocation destination = pSource;
            switch (pSource.Type.Primitive)
            {
                case LLPrimitive.Signed:
                    {
                        switch (pType.Primitive)
                        {
                            case LLPrimitive.Signed: // Signed to Signed
                                {
                                    if (pSource.Type.SizeInBits != pType.SizeInBits) destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    if (pSource.Type.SizeInBits > pType.SizeInBits) EmitTruncate(destination, pSource);
                                    else if (pSource.Type.SizeInBits < pType.SizeInBits) EmitExtend(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Unsigned: // Signed to Unsigned
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType)); // TODO: Problem here, i32 to u32 creates var, but doesn't assign it
                                    if (pSource.Type.SizeInBits > pType.SizeInBits) EmitTruncate(destination, pSource);
                                    else if (pSource.Type.SizeInBits < pType.SizeInBits) EmitExtend(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Float: // Signed to Float
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToFloat(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Pointer: // Signed to Pointer
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToPointer(destination, pSource);
                                    break;
                                }
                            default: throw new NotSupportedException();
                        }
                        break;
                    }
                case LLPrimitive.Unsigned:
                    {
                        switch (pType.Primitive)
                        {
                            case LLPrimitive.Signed: // Unsigned to Signed
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    if (pSource.Type.SizeInBits > pType.SizeInBits) EmitTruncate(destination, pSource);
                                    else if (pSource.Type.SizeInBits < pType.SizeInBits) EmitExtend(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Unsigned: // Unsigned to Unsigned
                                {
                                    if (pSource.Type.SizeInBits != pType.SizeInBits) destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    if (pSource.Type.SizeInBits > pType.SizeInBits) EmitTruncate(destination, pSource);
                                    else if (pSource.Type.SizeInBits < pType.SizeInBits) EmitExtend(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Float: // Unsigned to Float
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToFloat(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Pointer: // Unsigned to Pointer
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToPointer(destination, pSource);
                                    break;
                                }
                            default: throw new NotSupportedException();
                        }
                        break;
                    }
                case LLPrimitive.Float:
                    {
                        switch (pType.Primitive)
                        {
                            case LLPrimitive.Signed: // Float to Signed
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitFloatToInteger(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Unsigned: // Float to Unsigned
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitFloatToInteger(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Float: // Float to Float
                                {
                                    if (pSource.Type.SizeInBits != pType.SizeInBits) destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    if (pSource.Type.SizeInBits > pType.SizeInBits) EmitTruncate(destination, pSource);
                                    else if (pSource.Type.SizeInBits < pType.SizeInBits) EmitExtend(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Pointer: // Float to Pointer
                                {
                                    LLLocation temporaryInteger = LLTemporaryLocation.Create(Function.CreateTemporary(LLModule.GetOrCreateUnsignedType(pType.SizeInBits)));
                                    EmitFloatToInteger(temporaryInteger, pSource);
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToPointer(destination, temporaryInteger);
                                    break;
                                }
                            default: throw new NotSupportedException();
                        }
                        break;
                    }
                case LLPrimitive.Pointer:
                    {
                        switch (pType.Primitive)
                        {
                            case LLPrimitive.Signed: // Pointer to Signed
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitPointerToInteger(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Unsigned: // Pointer to Unsigned
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitPointerToInteger(destination, pSource);
                                    break;
                                }
                            case LLPrimitive.Float: // Pointer to Float
                                {
                                    LLLocation temporaryInteger = LLTemporaryLocation.Create(Function.CreateTemporary(LLModule.GetOrCreateUnsignedType(pSource.Type.SizeInBits)));
                                    EmitPointerToInteger(temporaryInteger, pSource);
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitIntegerToFloat(destination, temporaryInteger);
                                    break;
                                }
                            case LLPrimitive.Pointer: // Pointer to Pointer
                                {
                                    destination = LLTemporaryLocation.Create(Function.CreateTemporary(pType));
                                    EmitBitcast(destination, pSource);
                                    break;
                                }
                            default: throw new NotSupportedException();
                        }
                        break;
                    }
                default: throw new NotSupportedException();
            }
            return destination;
        }
    }
}
