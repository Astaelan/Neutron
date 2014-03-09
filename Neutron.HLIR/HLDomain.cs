using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.Immutable;
using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public static class HLDomain
    {
        private static IMetadataHost sHost = new PeReader.DefaultHost();
        private static IModule sModule = null;
        private static IPlatformType sPlatformType = null;
        private static IAssembly sRuntimeAssembly = null;
        private static HLMethod sEntryMethod = null;

        public static IMetadataHost Host { get { return sHost; } }
        public static IModule Module { get { return sModule; } }
        public static IPlatformType PlatformType { get { return sPlatformType; } }
        public static IAssembly RuntimeAssembly { get { return sRuntimeAssembly; } }
        public static HLMethod EntryMethod { get { return sEntryMethod; } }

        public static int SizeOfPointer { get { return PlatformType.PointerSize; } }

        private static Dictionary<string, HLType> sTypes = new Dictionary<string, HLType>();
        private static Dictionary<string, HLField> sFields = new Dictionary<string, HLField>();
        private static Dictionary<string, HLMethod> sMethods = new Dictionary<string, HLMethod>();
        private static Dictionary<string, HLParameter> sParameters = new Dictionary<string, HLParameter>();
        private static Dictionary<string, HLLocal> sLocals = new Dictionary<string, HLLocal>();

        private static HLType sSystemRuntimeMethodHandle = null;
        public static HLType SystemRuntimeMethodHandle { get { return sSystemRuntimeMethodHandle; } }

        private static HLType sSystemRuntimeType = null;
        public static HLType SystemRuntimeType { get { return sSystemRuntimeType; } }

        private static HLType sSystemRuntimeTypeData = null;
        public static HLType SystemRuntimeTypeData { get { return sSystemRuntimeTypeData; } }

        private static HLType sSystemRuntimeTypeHandle = null;
        public static HLType SystemRuntimeTypeHandle { get { return sSystemRuntimeTypeHandle; } }

        private static HLType sSystemString = null;
        public static HLType SystemString { get { return sSystemString; } }

        private static HLMethod sSystemStringCtorWithCharPointer = null;
        public static HLMethod SystemStringCtorWithCharPointer { get { return sSystemStringCtorWithCharPointer; } }

        private static Queue<HLMethod> sPendingMethods = new Queue<HLMethod>();

        private static LLFunction sGCAllocateFunction = null;
        public static LLFunction GCAllocFunction { get { return sGCAllocateFunction; } }

        private static LLFunction sGCRootFunction = null;
        public static LLFunction GCRootFunction { get { return sGCRootFunction; } }

        private static LLFunction sVTableLookupFunction = null;
        public static LLFunction VTableLookupFunction { get { return sVTableLookupFunction; } }

        public static void Process(string pInputPath)
        {
            sModule = Decompiler.GetCodeModelFromMetadataModel(Host, Host.LoadUnitFrom(pInputPath) as IModule, null);
            sPlatformType = Module.PlatformType;
            sRuntimeAssembly = (IAssembly)PlatformType.SystemRuntimeTypeHandle.ContainingUnitNamespace.Unit.ResolvedUnit;

            EnlistRuntimeTypes();

            EnlistRuntimeMethods();

            sEntryMethod = GetOrCreateMethod(Module.EntryPoint);

            int checkedTypes = 0;
            while (sTypes.Count != checkedTypes || sPendingMethods.Count > 0)
            {
                // STARTED: Process IStatements/IExpressions into HLInstructionBlocks/HLInstructions
                while (sPendingMethods.Count > 0) sPendingMethods.Dequeue().Process();

                // STARTED: Build virtual map for each type and include any missing override methods
                // starting at the top the chain (Object) and working down to the given type, then process
                // pending methods and repeat until no methods or types are added, as each new method may
                // add new types and methods
                checkedTypes = sTypes.Count;
                foreach (HLType type in sTypes.Values.ToList()) type.MapVirtualMethods();
            }
            // NOTE: After this point, do not GetOrCreate types and methods

            // DONE: Build virtual table and virtual index lookup from the complete virtual maps
            foreach (HLType type in sTypes.Values) type.BuildVirtualTable();

            // DONE: Determine HLType Calculated and Variable sizes
            // DONE: Layout Non-Static HLField offsets
            foreach (HLType type in sTypes.Values) type.LayoutFields();

            // NOTE: After this point, conversion to LL begins

            // DONE: Convert HLTypes to LLTypes

            BuildGCAllocateFunction();
            BuildGCRootFunction();
            BuildVTableLookupFunction();

            BuildGlobals();

            foreach (HLMethod method in sMethods.Values.ToList()) method.BuildFunction();

            // STARTED: Build constant global runtime type handles
            BuildRuntimeTypeHandles();

            // STARTED: Build constant global runtime type data
            BuildRuntimeTypeData();

            // STARTED: Convert HLInstructions to LLInstructions
            foreach (HLMethod method in sMethods.Values) method.Transform();

            using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(pInputPath, ".ll")))
            {
                LLModule.Dump(writer);
            }
        }


        private static HLType CreateType(ITypeDefinition pDefinition)
        {
            HLType type = new HLType();
            type.Definition = pDefinition;
            type.Name = pDefinition.ToString(); // TODO: Don't really like this, should use TypeHelper perhaps?
            type.Signature = HLDomain.GetTypeSignature(pDefinition);
            sTypes[type.Signature] = type;
            
            ITypeReference referenceBase = pDefinition.BaseClasses.FirstOrDefault();
            if (referenceBase != null)
            {
                type.BaseType = GetOrCreateType(referenceBase);
                type.Fields.AddRange(type.BaseType.MemberFields);
            }

            foreach (IFieldDefinition fieldDefinition in pDefinition.Fields.OrderBy(d => d.SequenceNumber))
                CreateField(type, fieldDefinition);

            IMethodDefinition definitionStaticConstructor = pDefinition.Methods.FirstOrDefault(d => d.IsStaticConstructor);
            if (definitionStaticConstructor != null) type.StaticConstructor = GetOrCreateMethod(definitionStaticConstructor);
            return type;
        }
        private static string GetTypeSignature(ITypeDefinition pDefinition) { return pDefinition.ToString(); }
        public static HLType GetOrCreateType(ITypeReference pReference) { return GetOrCreateType(pReference.ResolvedType); }
        public static HLType GetOrCreateType(ITypeDefinition pDefinition)
        {
            HLType type = null;
            if (!sTypes.TryGetValue(GetTypeSignature(pDefinition), out type)) type = CreateType(pDefinition);
            return type;
        }


        private static HLField CreateField(HLType pContainer, IFieldDefinition pDefinition)
        {
            HLField field = new HLField();
            //field.Definition = pDefinition;
            field.Name = pDefinition.Name.Value;
            field.Signature = HLDomain.GetFieldSignature(pDefinition);
            sFields[field.Signature] = field;

            field.Container = pContainer;
            pContainer.Fields.Add(field);

            field.IsStatic = pDefinition.IsStatic;
            field.IsCompileTimeConstant = pDefinition.IsCompileTimeConstant;
            field.Type = GetOrCreateType(pDefinition.Type);
            return field;
        }
        private static string GetFieldSignature(IFieldDefinition pDefinition) { return pDefinition.ToString(); }
        public static HLField GetField(IFieldReference pReference) { return GetField(pReference.ResolvedField); }
        public static HLField GetField(IFieldDefinition pDefinition) { return sFields[GetFieldSignature(pDefinition)]; }


        private static HLMethod CreateMethod(HLType pContainer, IMethodDefinition pDefinition)
        {
            HLMethod method = new HLMethod();
            method.Definition = pDefinition;
            method.Name = pDefinition.Name.Value;
            method.Signature = GetMethodSignature(pDefinition);
            sMethods[method.Signature] = method;

            method.Container = pContainer;
            pContainer.Methods.Add(method);

            method.IsStatic = pDefinition.IsStatic;
            method.IsExternal = pDefinition.IsExternal;
            method.IsAbstract = pDefinition.IsAbstract;
            method.IsVirtual = pDefinition.IsVirtual;
            method.IsConstructor = pDefinition.IsConstructor;
            method.ReturnType = GetOrCreateType(pDefinition.Type);
            if (!pDefinition.IsStatic && !pDefinition.HasExplicitThisParameter)
            {
                HLType typeImplicitThis = method.Container;
                if (typeImplicitThis.Definition.IsValueType)
                    typeImplicitThis = GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pDefinition.Container, Host.InternFactory, pDefinition.Container));
                method.Parameters.Add(new HLParameter() { Name = "this", Type = typeImplicitThis });
            }
            foreach (IParameterDefinition definition in pDefinition.Parameters.OrderBy(d => d.Index))
                method.Parameters.Add(CreateParameter(definition));
            foreach (ILocalDefinition definition in pDefinition.Body.LocalVariables.Where(d => d.Name.Value != string.Empty))
                method.Locals.Add(CreateLocal(definition));
            // TODO: Finish loading from Definition

            sPendingMethods.Enqueue(method);
            return method;
        }
        private static string GetMethodSignature(IMethodDefinition pDefinition)
        {
            // TODO: Fix for ByReference parameters that may make this not unique
            return pDefinition.ToString();
        }
        public static HLMethod GetOrCreateMethod(IMethodReference pReference) { return GetOrCreateMethod(pReference.ResolvedMethod); }
        public static HLMethod GetOrCreateMethod(IMethodDefinition pDefinition)
        {
            HLMethod method = null;
            if (!sMethods.TryGetValue(GetMethodSignature(pDefinition), out method))
            {
                HLType container = GetOrCreateType(pDefinition.Container);
                method = CreateMethod(container, pDefinition);
            }
            return method;
        }


        private static HLParameter CreateParameter(IParameterDefinition pDefinition)
        {
            HLParameter parameter = new HLParameter();
            //parameter.Definition = pDefinition;
            parameter.Name = pDefinition.Name.Value;
            parameter.Signature = HLDomain.GetParameterSignature(pDefinition);
            sParameters[parameter.Signature] = parameter;

            //if (pDefinition.ContainingSignature is IMethodDefinition) parameter.MethodContainer = GetOrCreateMethod(pDefinition.ContainingSignature as IMethodDefinition);
            //else throw new NotSupportedException();
            ITypeReference type = pDefinition.Type;
            if (pDefinition.IsByReference) type = MutableModelHelper.GetManagedPointerTypeReference(type, Host.InternFactory, type);
            parameter.IsReference = pDefinition.IsByReference;
            parameter.Type = GetOrCreateType(type);
            return parameter;
        }
        private static string GetParameterSignature(IParameterDefinition pDefinition)
        {
            // TODO: if pDefinition.IsByReference
            string signatureContainer = null;
            if (pDefinition.ContainingSignature is IMethodDefinition) signatureContainer = GetMethodSignature(pDefinition.ContainingSignature as IMethodDefinition);
            else throw new NotSupportedException();
            return signatureContainer + "|" + pDefinition.ToString();
        }
        public static HLParameter GetParameter(IParameterDefinition pDefinition) { return sParameters[GetParameterSignature(pDefinition)]; }


        private static HLLocal CreateLocal(ILocalDefinition pDefinition)
        {
            HLLocal local = new HLLocal();
            //local.Definition = pDefinition;
            local.Name = pDefinition.Name.Value;
            local.Signature = HLDomain.GetLocalSignature(pDefinition);
            sLocals[local.Signature] = local;

            //local.Container = GetOrCreateMethod(pDefinition.MethodDefinition);
            ITypeReference type = pDefinition.Type;
            if (pDefinition.IsReference) type = MutableModelHelper.GetManagedPointerTypeReference(type, Host.InternFactory, type);
            local.IsReference = pDefinition.IsReference;
            local.Type = GetOrCreateType(type);
            return local;
        }
        private static string GetLocalSignature(ILocalDefinition pDefinition)
        {
            // TODO: if pDefinition.IsReference
            
            string signatureContainer = GetMethodSignature(pDefinition.MethodDefinition);
            return signatureContainer + "|" + pDefinition.ToString();
        }
        public static HLLocal GetLocal(ILocalDefinition pDefinition) { return sLocals[GetLocalSignature(pDefinition)]; }


        private static void EnlistRuntimeTypes()
        {
            sSystemRuntimeMethodHandle = GetOrCreateType(PlatformType.SystemRuntimeMethodHandle);
            sSystemRuntimeType = GetOrCreateType(UnitHelper.FindType(Host.NameTable, sRuntimeAssembly, "System.RuntimeType"));
            sSystemRuntimeTypeData = GetOrCreateType(UnitHelper.FindType(Host.NameTable, sRuntimeAssembly, "System.RuntimeTypeData"));
            sSystemRuntimeTypeHandle = GetOrCreateType(PlatformType.SystemRuntimeTypeHandle);
            sSystemString = GetOrCreateType(PlatformType.SystemString);
            if (sSystemRuntimeTypeData.Definition.TypeCode == PrimitiveTypeCode.Invalid) throw new NotImplementedException();
        }

        private static void EnlistRuntimeMethods()
        {
            foreach (IMethodDefinition method in PlatformType.SystemString.ResolvedType.Methods.Where(d => d.IsConstructor && d.ParameterCount == 1))
            {
                IParameterDefinition parameter = method.Parameters.First();
                if (!(parameter.Type is IPointerTypeReference)) continue;
                if (((IPointerTypeReference)parameter.Type).TargetType.TypeCode != PrimitiveTypeCode.Char) continue;
                sSystemStringCtorWithCharPointer = GetOrCreateMethod(method);
                break;
            }
            if (sSystemStringCtorWithCharPointer == null) throw new MissingMethodException();
        }

        private static void BuildGCAllocateFunction()
        {
            List<LLParameter> parametersFunction = new List<LLParameter>();
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 2), "this"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreateUnsignedType(32), "size"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreateSignedType(32), "handle"));
            sGCAllocateFunction = LLModule.GetOrCreateFunction("GCAllocate", true, true, false, LLModule.VoidType, parametersFunction);
        }

        private static void BuildGCRootFunction()
        {
            List<LLParameter> parametersFunction = new List<LLParameter>();
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 2), "ptrloc"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1), "metadata"));
            sGCRootFunction = LLModule.GetOrCreateFunction("llvm.gcroot", true, true, false, LLModule.VoidType, parametersFunction);
        }

        private static void BuildVTableLookupFunction()
        {
            List<LLParameter> parametersFunction = new List<LLParameter>();
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1), "this"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreateSignedType(32), "index"));
            sVTableLookupFunction = LLModule.GetOrCreateFunction("VTableLookup", true, true, false, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1), parametersFunction);
        }

        private static void BuildGlobals()
        {
            foreach (HLType type in sTypes.Values)
            {
                // DONE: Create LLGlobal for static constructor runtime called check
                if (type.StaticConstructor != null) LLModule.CreateGlobal(LLModule.GetOrCreateUnsignedType(8).PointerDepthPlusOne, type.ToString());
                // DONE: Convert Static Non-Constant HLFields to LLGlobals
                foreach (HLField field in type.StaticFields.Where(f => !f.IsCompileTimeConstant))
                    LLModule.CreateGlobal(field.Type.LLType.PointerDepthPlusOne, type.ToString() + "." + field.ToString());
            }
        }

        private static void BuildRuntimeTypeHandles()
        {
            foreach (HLType type in sTypes.Values.OrderBy(t => t.RuntimeTypeHandle).Where(t => t.Definition.TypeCode != PrimitiveTypeCode.Pointer && t.Definition.TypeCode != PrimitiveTypeCode.Reference))
            {
                string globalName = "RuntimeTypeHandle_" + type.ToString().Replace(".", "");
                //int startOfPtrOrRef = globalName.IndexOfAny(new char[] { '*', '&' });
                //if (startOfPtrOrRef > 0) globalName = globalName.Insert(startOfPtrOrRef, "_");
                //globalName = globalName.Replace("*", "Ptr");
                //globalName = globalName.Replace("&", "Ref");
                LLGlobal globalHandle = LLModule.CreateGlobal(LLModule.GetOrCreateSignedType(32), globalName);
                globalHandle.InitialValue = LLLiteral.Create(LLModule.GetOrCreateSignedType(32), type.RuntimeTypeHandle.ToString());
            }
        }

        private static void BuildRuntimeTypeData()
        {
            List<LLType> fieldsRuntimeTypeData = sSystemRuntimeTypeData.Fields.Where(f => !f.IsStatic).ToList().ConvertAll(f => f.Type.LLType);
            LLType typePointer = LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
            LLType typeRuntimeTypeData = LLModule.GetOrCreateStructureType("RuntimeTypeData", true, fieldsRuntimeTypeData);
            LLType typeRuntimeTypeDataTable = LLModule.GetOrCreateArrayType(typeRuntimeTypeData, sTypes.Values.Count);
            LLGlobal globalRuntimeTypeDataTable = LLModule.CreateGlobal(typeRuntimeTypeDataTable.PointerDepthPlusOne, "RuntimeTypeDataTable");
            List<LLLiteral> literalsRuntimeTypeDataTable = new List<LLLiteral>();
            Dictionary<string, int> stringsRuntimeTypeDataStringTable = new Dictionary<string, int>();
            string stringRuntimeTypeDataStringTable = "";
            List<LLFunction> functionsRuntimeTypeDataVirtualTable = new List<LLFunction>();
            List<LLLiteral> literalsRuntimeTypeDataVirtualTable = new List<LLLiteral>();

            stringsRuntimeTypeDataStringTable[""] = stringRuntimeTypeDataStringTable.Length;
            stringRuntimeTypeDataStringTable += "\0";
            foreach (HLType type in sTypes.Values.OrderBy(t => t.RuntimeTypeHandle))
            {
                int flags = 0;
                if (type.Definition.IsValueType) flags |= 1 << 0;

                int offsetName = 0;
                string definitionName = TypeHelper.GetTypeName(type.Definition, NameFormattingOptions.OmitContainingNamespace);
                if (!stringsRuntimeTypeDataStringTable.TryGetValue(definitionName, out offsetName))
                {
                    offsetName = stringRuntimeTypeDataStringTable.Length;
                    stringsRuntimeTypeDataStringTable[definitionName] = offsetName;
                    stringRuntimeTypeDataStringTable += definitionName + "\0";
                }

                int offsetNamespace = 0;
                if (type.Definition is INamespaceTypeDefinition)
                {
                    string definitionNamespace = TypeHelper.GetNamespaceName(((INamespaceTypeDefinition)type.Definition).ContainingUnitNamespace, NameFormattingOptions.None);
                    if (!stringsRuntimeTypeDataStringTable.TryGetValue(definitionNamespace, out offsetNamespace))
                    {
                        offsetNamespace = stringRuntimeTypeDataStringTable.Length;
                        stringsRuntimeTypeDataStringTable[definitionNamespace] = offsetNamespace;
                        stringRuntimeTypeDataStringTable += definitionNamespace + "\0";
                    }
                }

                literalsRuntimeTypeDataTable.Add(typeRuntimeTypeData.ToLiteral(type.RuntimeTypeHandle.ToString(), flags.ToString(), type.CalculatedSize.ToString(), offsetName.ToString(), offsetNamespace.ToString(), functionsRuntimeTypeDataVirtualTable.Count.ToString()));

                List<LLFunction> functions = type.VirtualTable.ConvertAll(m => m.LLFunction);
                foreach (LLFunction function in functions)
                {
                    functionsRuntimeTypeDataVirtualTable.Add(function);
                    string literalAddress = null;
                    if (function.Abstract) literalAddress = "null";
                    else literalAddress = string.Format("bitcast({0} {1} to {2})", LLModule.GetOrCreatePointerType(function.FunctionType, 1), function, typePointer);
                    literalsRuntimeTypeDataVirtualTable.Add(LLLiteral.Create(typePointer, literalAddress));
                }
            }
            globalRuntimeTypeDataTable.InitialValue = typeRuntimeTypeDataTable.ToLiteral(literalsRuntimeTypeDataTable.ConvertAll(l => l.Value).ToArray());
            LLGlobal globalRuntimeTypeDataTableCount = LLModule.CreateGlobal(LLModule.GetOrCreateSignedType(32), "RuntimeTypeDataTableCount");
            globalRuntimeTypeDataTableCount.InitialValue = LLLiteral.Create(LLModule.GetOrCreateSignedType(32), sTypes.Values.Count.ToString());

            byte[] bufferRuntimeTypeDataStringTable = Encoding.ASCII.GetBytes(stringRuntimeTypeDataStringTable);
            string[] literalsRuntimeTypeDataStringTable = Array.ConvertAll(bufferRuntimeTypeDataStringTable, b => b.ToString());
            LLType typeRuntimeTypeDataStringTable = LLModule.GetOrCreateArrayType(LLModule.GetOrCreateSignedType(8), literalsRuntimeTypeDataStringTable.Length);
            LLGlobal globalRuntimeTypeDataStringTable = LLModule.CreateGlobal(typeRuntimeTypeDataStringTable.PointerDepthPlusOne, "RuntimeTypeDataStringTable");
            globalRuntimeTypeDataStringTable.InitialValue = typeRuntimeTypeDataStringTable.ToLiteral(literalsRuntimeTypeDataStringTable);

            LLType typeRuntimeTypeDataVirtualTable = LLModule.GetOrCreateArrayType(typePointer, functionsRuntimeTypeDataVirtualTable.Count);
            LLGlobal globalRuntimeTypeDataVirtualTable = LLModule.CreateGlobal(typeRuntimeTypeDataVirtualTable.PointerDepthPlusOne, "RuntimeTypeDataVirtualTable");
            globalRuntimeTypeDataVirtualTable.InitialValue = typeRuntimeTypeDataVirtualTable.ToLiteral(literalsRuntimeTypeDataVirtualTable.ConvertAll(l => l.Value).ToArray());

            LLGlobal globalRuntimeTypeDataVirtualTableCount = LLModule.CreateGlobal(LLModule.GetOrCreateSignedType(32), "RuntimeTypeDataVirtualTableCount");
            globalRuntimeTypeDataVirtualTableCount.InitialValue = LLLiteral.Create(LLModule.GetOrCreateSignedType(32), functionsRuntimeTypeDataVirtualTable.Count.ToString());
        }

        private static Lazy<HLType> sSystemArray = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemArray));
        public static HLType SystemArray { get { return sSystemArray.Value; } }

        private static Lazy<HLType> sSystemBoolean = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemBoolean));
        public static HLType SystemBoolean { get { return sSystemBoolean.Value; } }

        private static Lazy<HLType> sSystemByte = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemUInt8));
        public static HLType SystemByte { get { return sSystemByte.Value; } }

        private static Lazy<HLType> sSystemChar = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemChar));
        public static HLType SystemChar { get { return sSystemChar.Value; } }

        private static Lazy<HLType> sSystemDelegate = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemDelegate));
        public static HLType SystemDelegate { get { return sSystemDelegate.Value; } }

        private static Lazy<HLType> sSystemDouble = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemFloat64));
        public static HLType SystemDouble { get { return sSystemDouble.Value; } }

        private static Lazy<HLType> sSystemEnum = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemEnum));
        public static HLType SystemEnum { get { return sSystemEnum.Value; } }

        private static Lazy<HLType> sSystemException = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemException));
        public static HLType SystemException { get { return sSystemException.Value; } }

        private static Lazy<HLType> sSystemInt16 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemInt16));
        public static HLType SystemInt16 { get { return sSystemInt16.Value; } }

        private static Lazy<HLType> sSystemInt32 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemInt32));
        public static HLType SystemInt32 { get { return sSystemInt32.Value; } }

        private static Lazy<HLType> sSystemInt64 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemInt64));
        public static HLType SystemInt64 { get { return sSystemInt64.Value; } }

        private static Lazy<HLType> sSystemIntPtr = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemIntPtr));
        public static HLType SystemIntPtr { get { return sSystemIntPtr.Value; } }

        private static Lazy<HLType> sSystemMulticastDelegate = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemMulticastDelegate));
        public static HLType SystemMulticastDelegate { get { return sSystemMulticastDelegate.Value; } }

        private static Lazy<HLType> sSystemNullable = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemNullable));
        public static HLType SystemNullable { get { return sSystemNullable.Value; } }

        private static Lazy<HLType> sSystemObject = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemObject));
        public static HLType SystemObject { get { return sSystemObject.Value; } }

        private static Lazy<HLType> sSystemRuntimeArgumentHandle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemRuntimeArgumentHandle));
        public static HLType SystemRuntimeArgumentHandle { get { return sSystemRuntimeArgumentHandle.Value; } }

        private static Lazy<HLType> sSystemRuntimeFieldHandle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemRuntimeFieldHandle));
        public static HLType SystemRuntimeFieldHandle { get { return sSystemRuntimeFieldHandle.Value; } }

        private static Lazy<HLType> sSystemSByte = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemInt8));
        public static HLType SystemSByte { get { return sSystemSByte.Value; } }

        private static Lazy<HLType> sSystemSingle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemFloat32));
        public static HLType SystemSingle { get { return sSystemSingle.Value; } }

        private static Lazy<HLType> sSystemType = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemType));
        public static HLType SystemType { get { return sSystemType.Value; } }

        private static Lazy<HLType> sSystemTypedReference = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemTypedReference));
        public static HLType SystemTypedReference { get { return sSystemTypedReference.Value; } }

        private static Lazy<HLType> sSystemUInt16 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemUInt16));
        public static HLType SystemUInt16 { get { return sSystemUInt16.Value; } }

        private static Lazy<HLType> sSystemUInt32 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemUInt32));
        public static HLType SystemUInt32 { get { return sSystemUInt32.Value; } }

        private static Lazy<HLType> sSystemUInt64 = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemUInt64));
        public static HLType SystemUInt64 { get { return sSystemUInt64.Value; } }

        private static Lazy<HLType> sSystemUIntPtr = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemUIntPtr));
        public static HLType SystemUIntPtr { get { return sSystemUIntPtr.Value; } }

        private static Lazy<HLType> sSystemValueType = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemValueType));
        public static HLType SystemValueType { get { return sSystemValueType.Value; } }

        private static Lazy<HLType> sSystemVoid = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemVoid));
        public static HLType SystemVoid { get { return sSystemVoid.Value; } }
    }
}
