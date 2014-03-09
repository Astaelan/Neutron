using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Neutron.HLIR.Instructions;
using Neutron.HLIR.Locations;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLMethod
    {
        private static int sRuntimeMethodHandle = 0;

        internal HLMethod()
        {
            mRuntimeMethodHandle = sRuntimeMethodHandle++;
        }

        private int mRuntimeMethodHandle = 0;
        public int RuntimeMethodHandle { get { return mRuntimeMethodHandle; } }

        private IMethodDefinition mDefinition = null;
        public IMethodDefinition Definition { get { return mDefinition; } internal set { mDefinition = value; } }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        private string mSignature = null;
        public string Signature { get { return mSignature; } internal set { mSignature = value; } }

        private HLType mContainer = null;
        public HLType Container { get { return mContainer; } internal set { mContainer = value; } }

        private bool mIsStatic = false;
        public bool IsStatic { get { return mIsStatic; } internal set { mIsStatic = value; } }

        private bool mIsExternal = false;
        public bool IsExternal { get { return mIsExternal; } internal set { mIsExternal = value; } }

        private bool mIsAbstract = false;
        public bool IsAbstract { get { return mIsAbstract; } internal set { mIsAbstract = value; } }

        private bool mIsVirtual = false;
        public bool IsVirtual { get { return mIsVirtual; } internal set { mIsVirtual = value; } }

        private bool mIsConstructor = false;
        public bool IsConstructor { get { return mIsConstructor; } internal set { mIsConstructor = value; } }

        private HLType mReturnType = null;
        public HLType ReturnType { get { return mReturnType; } internal set { mReturnType = value; } }

        private List<HLParameter> mParameters = new List<HLParameter>();
        public List<HLParameter> Parameters { get { return mParameters; } }

        private List<HLLocal> mLocals = new List<HLLocal>();
        public List<HLLocal> Locals { get { return mLocals; } }

        private List<HLTemporary> mTemporaries = new List<HLTemporary>();
        public List<HLTemporary> Temporaries { get { return mTemporaries; } }

        private List<HLLabel> mLabels = new List<HLLabel>();
        public List<HLLabel> Labels { get { return mLabels; } }

        private Dictionary<string, HLLabel> mRemappedLabels = new Dictionary<string, HLLabel>();

        public HLLabel CreateLabel()
        {
            HLLabel label = HLLabel.Create(mLabels.Count);
            mLabels.Add(label);
            return label;
        }

        public HLTemporary CreateTemporary(HLType pType)
        {
            HLTemporary temporary = new HLTemporary();
            temporary.Name = "temp_" + Temporaries.Count;
            //temporary.Container = this;
            temporary.Type = pType;
            Temporaries.Add(temporary);
            return temporary;
        }

        private HLInstructionBlock mCurrentBlock = null;
        private List<HLInstructionBlock> mBlocks = new List<HLInstructionBlock>();
        public List<HLInstructionBlock> Blocks { get { return mBlocks; } }

        public HLInstructionBlock CreateBlock(HLLabel pStartLabel)
        {
            HLInstructionBlock block = HLInstructionBlock.Create(this, pStartLabel);
            mBlocks.Add(block);
            return block;
        }

        public override string ToString() { return Name; }

        internal void Process()
        {
            if (Definition.IsExternal) return;
            ISourceMethodBody sourceMethodBody = null;
            if (Definition.Body is ISourceMethodBody) sourceMethodBody = (ISourceMethodBody)Definition.Body;
            else sourceMethodBody = Decompiler.GetCodeModelFromMetadataModel(HLDomain.Host, Definition.Body, null);
            
            mCurrentBlock = CreateBlock(CreateLabel());
            ProcessStatement(sourceMethodBody.Block);
            mCurrentBlock.Terminate(null);

            List<string> buf = new List<string>();
            foreach (HLInstructionBlock block in mBlocks)
                foreach (HLInstruction instruction in block.Instructions)
                    buf.Add(instruction.ToString());
        }

        private void ProcessStatement(IStatement pStatement)
        {
            if (pStatement is IBlockStatement) ProcessBlockStatement(pStatement as IBlockStatement);
            else if (pStatement is IConditionalStatement) ProcessConditionalStatement(pStatement as IConditionalStatement);
            else if (pStatement is IEmptyStatement) ProcessEmptyStatement(pStatement as IEmptyStatement);
            else if (pStatement is IExpressionStatement) ProcessExpressionStatement(pStatement as IExpressionStatement);
            else if (pStatement is IGotoStatement) ProcessGotoStatement(pStatement as IGotoStatement);
            else if (pStatement is ILabeledStatement) ProcessLabeledStatement(pStatement as ILabeledStatement);
            else if (pStatement is ILocalDeclarationStatement) ProcessLocalDeclarationStatement(pStatement as ILocalDeclarationStatement);
            else if (pStatement is IReturnStatement) ProcessReturnStatement(pStatement as IReturnStatement);
            else if (pStatement is ISwitchStatement) ProcessSwitchStatement(pStatement as ISwitchStatement);
            else throw new NotSupportedException();
        }

        private void ProcessBlockStatement(IBlockStatement pStatement)
        {
            foreach (IStatement statement in pStatement.Statements)
                ProcessStatement(statement);
        }

        private void ProcessConditionalStatement(IConditionalStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            HLLocation locationCondition = ProcessExpression(pStatement.Condition);
            HLInstructionBlock blockParent = mCurrentBlock;

            HLInstructionBlock blockTrueStart = CreateBlock(CreateLabel());
            mCurrentBlock = blockTrueStart;
            ProcessStatement(pStatement.TrueBranch);
            HLInstructionBlock blockTrueEnd = mCurrentBlock;

            HLInstructionBlock blockFalseStart = CreateBlock(CreateLabel());
            mCurrentBlock = blockFalseStart;
            ProcessStatement(pStatement.FalseBranch);
            HLInstructionBlock blockFalseEnd = mCurrentBlock;

            blockParent.EmitBranch(locationCondition, blockTrueStart.StartLabel, blockFalseStart.StartLabel);

            if (!blockTrueEnd.Terminated || !blockFalseEnd.Terminated)
            {
                mCurrentBlock = CreateBlock(CreateLabel());
                blockTrueEnd.Terminate(mCurrentBlock.StartLabel);
                blockFalseEnd.Terminate(mCurrentBlock.StartLabel);
            }
        }

        private void ProcessEmptyStatement(IEmptyStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());
        }

        private void ProcessExpressionStatement(IExpressionStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            ProcessExpression(pStatement.Expression);
        }

        private void ProcessGotoStatement(IGotoStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            HLLabel labelTarget = null;
            if (!mRemappedLabels.TryGetValue(pStatement.TargetStatement.Label.Value, out labelTarget))
            {
                labelTarget = CreateLabel();
                mRemappedLabels.Add(pStatement.TargetStatement.Label.Value, labelTarget);
            }
            mCurrentBlock.EmitGoto(labelTarget);
        }

        private void ProcessLabeledStatement(ILabeledStatement pStatement)
        {
            HLLabel labelTarget = null;
            if (!mRemappedLabels.TryGetValue(pStatement.Label.Value, out labelTarget))
            {
                labelTarget = CreateLabel();
                mRemappedLabels.Add(pStatement.Label.Value, labelTarget);
            }
            mCurrentBlock.Terminate(labelTarget);
            mCurrentBlock = CreateBlock(labelTarget);
            ProcessStatement(pStatement.Statement);
        }

        private void ProcessLocalDeclarationStatement(ILocalDeclarationStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            if (pStatement.InitialValue == null) return;
            HLLocation locationLocalVariable = HLLocalLocation.Create(HLDomain.GetLocal(pStatement.LocalVariable));
            HLLocation locationInitialValue = ProcessExpression(pStatement.InitialValue);
            mCurrentBlock.EmitAssignment(locationLocalVariable, locationInitialValue);
        }

        private void ProcessReturnStatement(IReturnStatement pStatement)
        {
            if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            HLLocation locationExpression = null;
            if (pStatement.Expression != null) locationExpression = ProcessExpression(pStatement.Expression);
            mCurrentBlock.EmitReturn(locationExpression);
        }

        private void ProcessSwitchStatement(ISwitchStatement pStatement)
        {
            //if (mCurrentBlock.Terminated) mCurrentBlock = CreateBlock(CreateLabel());

            HLLocation locationCondition = ProcessExpression(pStatement.Expression);
            HLInstructionBlock blockParent = mCurrentBlock;

            List<HLInstructionBlock> blocksStarts = new List<HLInstructionBlock>();
            List<HLInstructionBlock> blocksEnds = new List<HLInstructionBlock>();
            HLInstructionBlock blockDefaultCase = null;
            List<Tuple<HLLiteralLocation, HLLabel>> cases = new List<Tuple<HLLiteralLocation, HLLabel>>();
            foreach (ISwitchCase switchCase in pStatement.Cases)
            {
                HLInstructionBlock blockCase = CreateBlock(CreateLabel());
                mCurrentBlock = blockCase;
                blocksStarts.Add(blockCase);
                if (switchCase.IsDefault) blockDefaultCase = blockCase;
                else
                {
                    HLLiteralLocation locationCase = (HLLiteralLocation)ProcessCompileTimeConstantExpression(switchCase.Expression);
                    cases.Add(new Tuple<HLLiteralLocation, HLLabel>(locationCase, blockCase.StartLabel));
                }
                foreach (IStatement statementCase in switchCase.Body)
                    ProcessStatement(statementCase);
                blocksEnds.Add(mCurrentBlock);
            }
            if (blockDefaultCase == null)
            {
                blockDefaultCase = CreateBlock(CreateLabel());
                mCurrentBlock = blockDefaultCase;
                blocksStarts.Add(blockDefaultCase);
                blocksEnds.Add(blockDefaultCase);
            }

            blockParent.EmitSwitch(locationCondition, blockDefaultCase.StartLabel, cases);

            if (!blocksEnds.TrueForAll(b => b.Terminated))
            {
                mCurrentBlock = CreateBlock(CreateLabel());
                blocksEnds.ForEach(b => b.Terminate(mCurrentBlock.StartLabel));
            }
        }


        private HLLocation ProcessExpression(IExpression pExpression)
        {
            if (pExpression is IAddition) return ProcessAdditionExpression(pExpression as IAddition);
            if (pExpression is IAddressableExpression) return ProcessAddressableExpression(pExpression as IAddressableExpression);
            if (pExpression is IAddressDereference) return ProcessAddressDereferenceExpression(pExpression as IAddressDereference);
            if (pExpression is IAddressOf) return ProcessAddressOfExpression(pExpression as IAddressOf);
            if (pExpression is IArrayIndexer) return ProcessArrayIndexerExpression(pExpression as IArrayIndexer);
            if (pExpression is IAssignment) return ProcessAssignmentExpression(pExpression as IAssignment);
            if (pExpression is IBitwiseAnd) return ProcessBitwiseAndExpression(pExpression as IBitwiseAnd);
            if (pExpression is IBitwiseOr) return ProcessBitwiseOrExpression(pExpression as IBitwiseOr);
            if (pExpression is IBoundExpression) return ProcessBoundExpression(pExpression as IBoundExpression);
            if (pExpression is ICompileTimeConstant) return ProcessCompileTimeConstantExpression(pExpression as ICompileTimeConstant);
            if (pExpression is IConditional) return ProcessConditionalExpression(pExpression as IConditional);
            if (pExpression is IConversion) return ProcessConversionExpression(pExpression as IConversion);
            if (pExpression is ICreateArray) return ProcessCreateArrayExpression(pExpression as ICreateArray);
            if (pExpression is ICreateDelegateInstance) return ProcessCreateDelegateInstanceExpression(pExpression as ICreateDelegateInstance);
            if (pExpression is ICreateObjectInstance) return ProcessCreateObjectInstanceExpression(pExpression as ICreateObjectInstance);
            if (pExpression is IDefaultValue) return ProcessDefaultValueExpression(pExpression as IDefaultValue);
            if (pExpression is IDivision) return ProcessDivisionExpression(pExpression as IDivision);
            if (pExpression is IEquality) return ProcessEqualityExpression(pExpression as IEquality);
            if (pExpression is IExclusiveOr) return ProcessExclusiveOrExpression(pExpression as IExclusiveOr);
            if (pExpression is IGreaterThan) return ProcessGreaterThanExpression(pExpression as IGreaterThan);
            if (pExpression is IGreaterThanOrEqual) return ProcessGreaterThanOrEqualExpression(pExpression as IGreaterThanOrEqual);
            if (pExpression is ILessThan) return ProcessLessThanExpression(pExpression as ILessThan);
            if (pExpression is ILessThanOrEqual) return ProcessLessThanOrEqualExpression(pExpression as ILessThanOrEqual);
            if (pExpression is ILogicalNot) return ProcessLogicalNotExpression(pExpression as ILogicalNot);
            if (pExpression is IMethodCall) return ProcessMethodCallExpression(pExpression as IMethodCall);
            if (pExpression is IModulus) return ProcessModulusExpression(pExpression as IModulus);
            if (pExpression is IMultiplication) return ProcessMultiplicationExpression(pExpression as IMultiplication);
            if (pExpression is INotEquality) return ProcessNotEqualityExpression(pExpression as INotEquality);
            if (pExpression is IOnesComplement) return ProcessOnesComplementExpression(pExpression as IOnesComplement);
            if (pExpression is ISizeOf) return ProcessSizeOfExpression(pExpression as ISizeOf);
            if (pExpression is ISubtraction) return ProcessSubtractionExpression(pExpression as ISubtraction);
            if (pExpression is ITargetExpression) return ProcessTargetExpression(pExpression as ITargetExpression);
            if (pExpression is IThisReference) return ProcessThisReferenceExpression(pExpression as IThisReference);
            if (pExpression is ITypeOf) return ProcessTypeOfExpression(pExpression as ITypeOf);
            if (pExpression is IUnaryNegation) return ProcessUnaryNegationExpression(pExpression as IUnaryNegation);
            if (pExpression is IVectorLength) return ProcessVectorLengthExpression(pExpression as IVectorLength);
            throw new NotSupportedException();
        }

        private HLLocation ProcessAdditionExpression(IAddition pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitAdd(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessAddressableExpression(IAddressableExpression pExpression)
        {
            if (pExpression.Definition is ILocalDefinition) return HLLocalLocation.Create(HLDomain.GetLocal(pExpression.Definition as ILocalDefinition));
            else if (pExpression.Definition is IParameterDefinition) return HLParameterLocation.Create(HLDomain.GetParameter(pExpression.Definition as IParameterDefinition));
            else if (pExpression.Definition is IFieldDefinition || pExpression.Definition is IFieldReference)
            {
                IFieldDefinition definition = pExpression.Definition is IFieldDefinition ? (IFieldDefinition)pExpression.Definition : ((IFieldReference)pExpression.Definition).ResolvedField;
                HLType typeFieldContainer = HLDomain.GetOrCreateType(definition.Container);
                HLField field = HLDomain.GetField(definition);
                if (field.IsStatic) return HLStaticFieldLocation.Create(field);
                HLLocation instance = ProcessExpression(pExpression.Instance);
                return HLFieldLocation.Create(instance, field);
            }
            else if (pExpression.Definition is IArrayIndexer)
            {
                IArrayIndexer definition = (IArrayIndexer)pExpression.Definition;
                if (definition.Indices.Count() != 1) throw new NotSupportedException();
                HLLocation locationInstance = ProcessExpression(pExpression.Instance);
                HLLocation locationIndex = ProcessExpression(definition.Indices.First());
                HLType typeElement = HLDomain.GetOrCreateType(pExpression.Type);
                return HLArrayElementLocation.Create(locationInstance, locationIndex, typeElement);
            }
            else if (pExpression.Definition is IDefaultValue)
            {
                HLType typeDefaultValue = HLDomain.GetOrCreateType(((IDefaultValue)pExpression.Definition).DefaultValueType);
                HLLocation locationDefaultValue = HLTemporaryLocation.Create(CreateTemporary(typeDefaultValue));
                mCurrentBlock.EmitAssignment(locationDefaultValue, HLDefaultLocation.Create(typeDefaultValue));
                return locationDefaultValue;
            }
            throw new NotSupportedException();
        }

        private HLLocation ProcessAddressDereferenceExpression(IAddressDereference pExpression)
        {
            HLLocation locationAddress = ProcessExpression(pExpression.Address);
            return HLIndirectAddressLocation.Create(locationAddress, HLDomain.GetOrCreateType(pExpression.Type));
        }

        private HLLocation ProcessAddressOfExpression(IAddressOf pExpression)
        {
            return ProcessExpression(pExpression.Expression).AddressOf();
        }

        private HLLocation ProcessArrayIndexerExpression(IArrayIndexer pExpression)
        {
            if (pExpression.Indices.Count() != 1) throw new NotSupportedException();

            HLLocation locationInstance = ProcessExpression(pExpression.IndexedObject);
            HLLocation locationIndex = ProcessExpression(pExpression.Indices.First());
            HLType typeElement = HLDomain.GetOrCreateType(pExpression.Type);
            return HLArrayElementLocation.Create(locationInstance, locationIndex, typeElement);
        }

        private HLLocation ProcessAssignmentExpression(IAssignment pExpression)
        {
            HLLocation locationSource = ProcessExpression(pExpression.Source);
            HLLocation locationTarget = ProcessTargetExpression(pExpression.Target);
            mCurrentBlock.EmitAssignment(locationTarget, locationSource);
            return locationTarget;
        }

        private HLLocation ProcessBitwiseAndExpression(IBitwiseAnd pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseAnd(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessBitwiseOrExpression(IBitwiseOr pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseOr(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessBoundExpression(IBoundExpression pExpression)
        {
            if (pExpression.Definition is ILocalDefinition)
            {
                ILocalDefinition definition = pExpression.Definition as ILocalDefinition;
                HLLocation location = HLLocalLocation.Create(HLDomain.GetLocal(definition));
                if (pExpression.Type.ResolvedType.TypeCode == PrimitiveTypeCode.Reference)
                    location = location.AddressOf();
                return location;
            }
            else if (pExpression.Definition is IParameterDefinition) return HLParameterLocation.Create(HLDomain.GetParameter(pExpression.Definition as IParameterDefinition));
            else if (pExpression.Definition is IFieldDefinition || pExpression.Definition is IFieldReference)
            {
                IFieldDefinition definition = pExpression.Definition is IFieldDefinition ? (IFieldDefinition)pExpression.Definition : ((IFieldReference)pExpression.Definition).ResolvedField;
                HLType typeFieldContainer = HLDomain.GetOrCreateType(definition.Container);
                HLField field = HLDomain.GetField(definition);
                if (field.IsStatic) return HLStaticFieldLocation.Create(field);
                HLLocation instance = ProcessExpression(pExpression.Instance);
                return HLFieldLocation.Create(instance, field);
            }
            throw new NotSupportedException();
        }

        private HLLocation ProcessCompileTimeConstantExpression(ICompileTimeConstant pExpression)
        {
            if (pExpression.Value == null) return HLNullLocation.Create(HLDomain.GetOrCreateType(pExpression.Type));
            if (pExpression.Value is bool) return HLBooleanLiteralLocation.Create((bool)pExpression.Value);
            if (pExpression.Value is sbyte) return HLInt8LiteralLocation.Create((sbyte)pExpression.Value);
            if (pExpression.Value is byte) return HLUInt8LiteralLocation.Create((byte)pExpression.Value);
            if (pExpression.Value is char) return HLCharLiteralLocation.Create((char)pExpression.Value);
            if (pExpression.Value is short) return HLInt16LiteralLocation.Create((short)pExpression.Value);
            if (pExpression.Value is ushort) return HLUInt16LiteralLocation.Create((ushort)pExpression.Value);
            if (pExpression.Value is float) return HLFloat32LiteralLocation.Create((float)pExpression.Value);
            if (pExpression.Value is int) return HLInt32LiteralLocation.Create((int)pExpression.Value);
            if (pExpression.Value is uint) return HLUInt32LiteralLocation.Create((uint)pExpression.Value);
            if (pExpression.Value is double) return HLFloat64LiteralLocation.Create((double)pExpression.Value);
            if (pExpression.Value is long) return HLInt64LiteralLocation.Create((long)pExpression.Value);
            if (pExpression.Value is ulong) return HLUInt64LiteralLocation.Create((ulong)pExpression.Value);
            if (pExpression.Value is string) return HLStringLiteralLocation.Create((string)pExpression.Value);
            throw new NotSupportedException();
        }

        private HLLocation ProcessConditionalExpression(IConditional pExpression)
        {
            HLLocation locationCondition = ProcessExpression(pExpression.Condition);
            HLInstructionBlock blockParent = mCurrentBlock;

            HLLocation locationResult = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));

            HLInstructionBlock blockTrueStart = CreateBlock(CreateLabel());
            mCurrentBlock = blockTrueStart;
            HLLocation locationTrue = ProcessExpression(pExpression.ResultIfTrue);
            mCurrentBlock.EmitAssignment(locationResult, locationTrue);
            HLInstructionBlock blockTrueEnd = mCurrentBlock;

            HLInstructionBlock blockFalseStart = CreateBlock(CreateLabel());
            mCurrentBlock = blockFalseStart;
            HLLocation locationFalse = ProcessExpression(pExpression.ResultIfFalse);
            mCurrentBlock.EmitAssignment(locationResult, locationFalse);
            HLInstructionBlock blockFalseEnd = mCurrentBlock;

            blockParent.EmitBranch(locationCondition, blockTrueStart.StartLabel, blockFalseStart.StartLabel);

            mCurrentBlock = CreateBlock(CreateLabel());
            blockTrueEnd.Terminate(mCurrentBlock.StartLabel);
            blockFalseEnd.Terminate(mCurrentBlock.StartLabel);

            return locationResult;
        }

        private HLLocation ProcessConversionExpression(IConversion pExpression)
        {
            HLLocation locationSource = ProcessExpression(pExpression.ValueToConvert);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.TypeAfterConversion)));
            mCurrentBlock.EmitAssignment(locationTemporary, locationSource);
            return locationTemporary;
        }

        private HLLocation ProcessCreateArrayExpression(ICreateArray pExpression)
        {
            if (pExpression.Rank != 1 || pExpression.Sizes.Count() != 1 || pExpression.LowerBounds.Count() > 0) throw new NotSupportedException();

            HLType typeElement = HLDomain.GetOrCreateType(pExpression.ElementType);
            HLLocation locationInstance = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            HLLocation locationSize = ProcessExpression(pExpression.Sizes.First());
            mCurrentBlock.EmitNewArray(locationInstance.AddressOf(), locationSize, typeElement);

            IExpression[] initializers = pExpression.Initializers.ToArray();
            for (int indexInitializer = 0; indexInitializer < initializers.Length; ++indexInitializer)
            {
                HLLocation locationInitializer = ProcessExpression(initializers[indexInitializer]);
                HLLocation locationArrayElement = HLArrayElementLocation.Create(locationInstance, HLInt32LiteralLocation.Create(indexInitializer), typeElement);
                mCurrentBlock.EmitAssignment(locationArrayElement, locationInitializer);
            }

            return locationInstance;
        }

        private HLLocation ProcessCreateDelegateInstanceExpression(ICreateDelegateInstance pExpression)
        {
            return null;
        }

        private HLLocation ProcessCreateObjectInstanceExpression(ICreateObjectInstance pExpression)
        {
            HLMethod methodCalled = HLDomain.GetOrCreateMethod(pExpression.MethodToCall);
            HLLocation locationThis = HLTemporaryLocation.Create(CreateTemporary(methodCalled.Container));
            List<HLLocation> locationsParameters = new List<HLLocation>();
            if (methodCalled.Container == HLDomain.SystemString) locationsParameters.Add(locationThis.AddressOf());
            else
            {
                mCurrentBlock.EmitNewObject(methodCalled.Container, locationThis.AddressOf());
                locationsParameters.Add(locationThis);
            }

            foreach (IExpression argument in pExpression.Arguments)
                locationsParameters.Add(ProcessExpression(argument));

            mCurrentBlock.EmitCall(methodCalled, false, null, locationsParameters);
            return locationThis;
        }

        private HLLocation ProcessDefaultValueExpression(IDefaultValue pExpression)
        {
            return HLDefaultLocation.Create(HLDomain.GetOrCreateType(pExpression.DefaultValueType));
        }

        private HLLocation ProcessDivisionExpression(IDivision pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitDivide(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessEqualityExpression(IEquality pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.Equal, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessExclusiveOrExpression(IExclusiveOr pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseXor(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessGreaterThanExpression(IGreaterThan pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.GreaterThan, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessGreaterThanOrEqualExpression(IGreaterThanOrEqual pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.GreaterThanOrEqual, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessLessThanExpression(ILessThan pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.LessThan, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessLessThanOrEqualExpression(ILessThanOrEqual pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.LessThanOrEqual, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessLogicalNotExpression(ILogicalNot pExpression)
        {
            HLLocation locationOperand = ProcessExpression(pExpression.Operand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseXor(locationTemporary, locationOperand, HLBooleanLiteralLocation.Create(true));
            return locationTemporary;
        }

        private HLLocation ProcessMethodCallExpression(IMethodCall pExpression)
        {
            List<HLLocation> locationsParameters = new List<HLLocation>();
            if (pExpression.ThisArgument.Type.TypeCode != PrimitiveTypeCode.Invalid)
                locationsParameters.Add(ProcessExpression(pExpression.ThisArgument));
            foreach (IExpression argument in pExpression.Arguments)
                locationsParameters.Add(ProcessExpression(argument));

            HLLocation locationReturn = null;
            HLMethod methodCalled = HLDomain.GetOrCreateMethod(pExpression.MethodToCall);
            if (methodCalled.ReturnType.Definition.TypeCode != PrimitiveTypeCode.Void)
                locationReturn = HLTemporaryLocation.Create(CreateTemporary(methodCalled.ReturnType));
            mCurrentBlock.EmitCall(methodCalled, pExpression.IsVirtualCall, locationReturn, locationsParameters);
            return locationReturn;
        }

        private HLLocation ProcessModulusExpression(IModulus pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitModulus(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessMultiplicationExpression(IMultiplication pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitMultiply(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessNotEqualityExpression(INotEquality pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitCompare(HLCompareType.NotEqual, locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessOnesComplementExpression(IOnesComplement pExpression)
        {
            HLLocation locationOperand = ProcessExpression(pExpression.Operand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseNot(locationTemporary, locationOperand);
            return locationTemporary;
        }

        private HLLocation ProcessSizeOfExpression(ISizeOf pExpression)
        {
            return HLSizeOfLocation.Create(HLDomain.GetOrCreateType(pExpression.TypeToSize));
        }

        private HLLocation ProcessSubtractionExpression(ISubtraction pExpression)
        {
            HLLocation locationLeftOperand = ProcessExpression(pExpression.LeftOperand);
            HLLocation locationRightOperand = ProcessExpression(pExpression.RightOperand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitSubtract(locationTemporary, locationLeftOperand, locationRightOperand);
            return pExpression.ResultIsUnmodifiedLeftOperand ? locationLeftOperand : locationTemporary;
        }

        private HLLocation ProcessTargetExpression(ITargetExpression pExpression)
        {
            if (pExpression.Definition is ILocalDefinition) return HLLocalLocation.Create(HLDomain.GetLocal(pExpression.Definition as ILocalDefinition));
            else if (pExpression.Definition is IParameterDefinition)
            {
                HLParameter parameter = HLDomain.GetParameter(pExpression.Definition as IParameterDefinition);
                parameter.RequiresAddressing = true;
                return HLParameterLocation.Create(parameter);
            }
            else if (pExpression.Definition is IFieldDefinition || pExpression.Definition is IFieldReference)
            {
                IFieldDefinition definition = pExpression.Definition is IFieldDefinition ? (IFieldDefinition)pExpression.Definition : ((IFieldReference)pExpression.Definition).ResolvedField;
                HLType typeFieldContainer = HLDomain.GetOrCreateType(definition.Container);
                HLField field = HLDomain.GetField(definition);
                if (field.IsStatic) return HLStaticFieldLocation.Create(field);
                HLLocation instance = ProcessExpression(pExpression.Instance);
                return HLFieldLocation.Create(instance, field);
            }
            else if (pExpression.Definition is IAddressDereference)
            {
                IAddressDereference definition = pExpression.Definition as IAddressDereference;
                HLLocation locationAddress = ProcessExpression(definition.Address);
                return HLIndirectAddressLocation.Create(locationAddress, HLDomain.GetOrCreateType(pExpression.Type));
            }
            throw new NotSupportedException();
        }

        private HLLocation ProcessThisReferenceExpression(IThisReference pExpression)
        {
            return HLParameterLocation.Create(Parameters[0]);
        }

        private HLLocation ProcessTypeOfExpression(ITypeOf pExpression)
        {
            return HLTypeOfLocation.Create(HLDomain.GetOrCreateType(pExpression.TypeToGet));
        }

        private HLLocation ProcessUnaryNegationExpression(IUnaryNegation pExpression)
        {
            HLLocation locationOperand = ProcessExpression(pExpression.Operand);
            HLLocation locationTemporary = HLTemporaryLocation.Create(CreateTemporary(HLDomain.GetOrCreateType(pExpression.Type)));
            mCurrentBlock.EmitBitwiseNegate(locationTemporary, locationOperand);
            return locationTemporary;
        }

        private HLLocation ProcessVectorLengthExpression(IVectorLength pExpression)
        {
            HLLocation locationInstance = ProcessExpression(pExpression.Vector);
            return HLArrayLengthLocation.Create(locationInstance);
        }



        private LLFunction mLLFunction = null;
        internal LLFunction LLFunction { get { return mLLFunction; } }

        internal void BuildFunction()
        {
            // DONE: Convert HLMethods to LLFunctions
            List<LLParameter> parameters = new List<LLParameter>();
            foreach (HLParameter parameter in Parameters)
            {
                LLType typeParameter = parameter.Type.LLType;
                // DONE: Adjust first parameter for string constructors to additional pointer depth plus one
                if (parameter == Parameters.First() && Container == HLDomain.SystemString && !IsStatic && IsConstructor)
                    typeParameter = typeParameter.PointerDepthPlusOne;
                parameters.Add(LLParameter.Create(typeParameter, parameter.Name));
            }
            bool entryFunction = HLDomain.EntryMethod == this;
            mLLFunction = LLModule.GetOrCreateFunction(entryFunction ? "main" : (Container.ToString() + "." + ToString()), entryFunction, IsExternal, IsAbstract, ReturnType.LLType, parameters);
            LLFunction.Description = Signature;
            foreach (HLParameter parameter in Parameters.Where(p => p.RequiresAddressing)) parameter.AddressableLocal = LLFunction.CreateLocal(parameter.Type.LLType, "local_" + parameter.Name);
            foreach (HLLocal local in Locals) LLFunction.CreateLocal(local.Type.LLType, local.Name);
            foreach (HLTemporary temporary in Temporaries) LLFunction.CreateLocal(temporary.Type.LLType, temporary.Name);
        }

        internal void Transform()
        {
            Labels.ForEach(l => LLFunction.CreateLabel(l.Identifier));

            foreach (HLInstructionBlock hl in Blocks)
            {
                LLInstructionBlock ll = LLFunction.CreateBlock(LLFunction.Labels.GetByIdentifier(hl.StartLabel.Identifier));
                LLFunction.CurrentBlock = ll;
                if (hl == Blocks.First())
                {
                    foreach (HLParameter parameter in Parameters.Where(p => p.RequiresAddressing))
                        LLFunction.CurrentBlock.EmitStore(LLLocalLocation.Create(parameter.AddressableLocal), LLParameterLocation.Create(LLFunction.Parameters[parameter.Name]));
                }

                hl.Instructions.ForEach(i => i.Transform(LLFunction));
            }
        }
    }
}
