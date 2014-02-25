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

        public static IMetadataHost Host { get { return sHost; } }
        public static IModule Module { get { return sModule; } }
        public static IPlatformType PlatformType { get { return sPlatformType; } }

        public static int SizeOfPointer { get { return PlatformType.PointerSize; } }

        private static Dictionary<string, HLType> sTypes = new Dictionary<string, HLType>();
        private static Dictionary<string, HLField> sFields = new Dictionary<string, HLField>();
        private static Dictionary<string, HLMethod> sMethods = new Dictionary<string, HLMethod>();
        private static Dictionary<string, HLParameter> sParameters = new Dictionary<string, HLParameter>();
        private static Dictionary<string, HLLocal> sLocals = new Dictionary<string, HLLocal>();

        private static HLMethod sSystemStringCtorWithCharPointer = null;
        public static HLMethod SystemStringCtorWithCharPointer { get { return sSystemStringCtorWithCharPointer; } }

        private static Queue<HLMethod> sPendingMethods = new Queue<HLMethod>();

        private static LLFunction sGCAllocateFunction = null;
        public static LLFunction GCAllocFunction { get { return sGCAllocateFunction; } }

        private static LLFunction sGCRootFunction = null;
        public static LLFunction GCRootFunction { get { return sGCRootFunction; } }

        public static void Process(string pInputPath)
        {
            sModule = Decompiler.GetCodeModelFromMetadataModel(Host, Host.LoadUnitFrom(pInputPath) as IModule, null);
            sPlatformType = Module.PlatformType;

            HLMethod methodEntry = GetOrCreateMethod(Module.EntryPoint);
            foreach (IMethodDefinition method in PlatformType.SystemString.ResolvedType.Methods.Where(d => d.IsConstructor && d.ParameterCount == 1))
            {
                IParameterDefinition parameter = method.Parameters.First();
                if (!(parameter.Type is IPointerTypeReference)) continue;
                if (((IPointerTypeReference)parameter.Type).TargetType.TypeCode != PrimitiveTypeCode.Char) continue;
                sSystemStringCtorWithCharPointer = GetOrCreateMethod(method);
                break;
            }
            if (sSystemStringCtorWithCharPointer == null) throw new MissingMethodException();

            // STARTED: Process IStatements/IExpressions into HLInstructionBlocks/HLInstructions
            while (sPendingMethods.Count > 0) sPendingMethods.Dequeue().Process();

            // DONE: Determine HLType Calculated and Variable sizes
            // DONE: Layout Non-Static HLField offsets
            foreach (HLType type in sTypes.Values) type.LayoutFields();

            // DONE: Convert HLTypes to LLTypes

            List<LLParameter> parametersFunction = new List<LLParameter>();
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 2), "this"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreateUnsignedType(32), "size"));
            sGCAllocateFunction = LLModule.GetOrCreateFunction("GCAllocate", true, true, LLModule.VoidType, parametersFunction);

            parametersFunction.Clear();
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 2), "ptrloc"));
            parametersFunction.Add(LLParameter.Create(LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1), "metadata"));
            sGCRootFunction = LLModule.GetOrCreateFunction("llvm.gcroot", true, true, LLModule.VoidType, parametersFunction);

            foreach (HLType type in sTypes.Values)
            {
                // DONE: Create LLGlobal for static constructors
                if (type.StaticConstructor != null) LLModule.CreateGlobal(LLModule.GetOrCreateUnsignedType(8).PointerDepthPlusOne, type.ToString());
                // DONE: Convert Static Non-Constant HLFields to LLGlobals
                foreach (HLField field in type.StaticFields.Where(f => !f.IsCompileTimeConstant))
                    LLModule.CreateGlobal(field.Type.LLType.PointerDepthPlusOne, type.ToString() + "." + field.ToString());
            }
            foreach (HLMethod method in sMethods.Values)
            {
                // DONE: Convert HLMethods to LLFunctions
                List<LLParameter> parameters = new List<LLParameter>();
                foreach (HLParameter parameter in method.Parameters)
                {
                    LLType typeParameter = parameter.Type.LLType;
                    // DONE: Adjust first parameter for string constructors to additional pointer depth plus one
                    if (parameter == method.Parameters.First() && method.Container == SystemString && !method.IsStatic && method.IsConstructor)
                        typeParameter = typeParameter.PointerDepthPlusOne;
                    parameters.Add(LLParameter.Create(typeParameter, parameter.Name));
                }
                bool entryFunction = method == methodEntry;
                method.LLFunction = LLModule.GetOrCreateFunction(entryFunction ? "main" : (method.Container.ToString() + "." + method.ToString()), entryFunction, method.IsExternal, method.ReturnType.LLType, parameters);
                method.LLFunction.Description = method.Signature;
                foreach (HLParameter parameter in method.Parameters.Where(p => p.RequiresAddressing)) parameter.AddressableLocal = method.LLFunction.CreateLocal(parameter.Type.LLType, "local_" + parameter.Name);
                foreach (HLLocal local in method.Locals) method.LLFunction.CreateLocal(local.Type.LLType, local.Name);
                foreach (HLTemporary temporary in method.Temporaries) method.LLFunction.CreateLocal(temporary.Type.LLType, temporary.Name);
            }
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
        private static string GetMethodSignature(IMethodDefinition pDefinition) { return pDefinition.ToString(); }
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

            parameter.Type = GetOrCreateType(pDefinition.Type);
            return parameter;
        }
        private static string GetParameterSignature(IParameterDefinition pDefinition)
        {
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

            local.Type = GetOrCreateType(pDefinition.Type);
            return local;
        }
        private static string GetLocalSignature(ILocalDefinition pDefinition)
        {
            string signatureContainer = GetMethodSignature(pDefinition.MethodDefinition);
            return signatureContainer + "|" + pDefinition.ToString();
        }
        public static HLLocal GetLocal(ILocalDefinition pDefinition) { return sLocals[GetLocalSignature(pDefinition)]; }


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

        private static Lazy<HLType> sSystemRuntimeMethodHandle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemRuntimeMethodHandle));
        public static HLType SystemRuntimeMethodHandle { get { return sSystemRuntimeMethodHandle.Value; } }

        private static Lazy<HLType> sSystemRuntimeTypeHandle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemRuntimeTypeHandle));
        public static HLType SystemRuntimeTypeHandle { get { return sSystemRuntimeTypeHandle.Value; } }

        private static Lazy<HLType> sSystemSByte = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemInt8));
        public static HLType SystemSByte { get { return sSystemSByte.Value; } }

        private static Lazy<HLType> sSystemSingle = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemFloat32));
        public static HLType SystemSingle { get { return sSystemSingle.Value; } }

        private static Lazy<HLType> sSystemString = new Lazy<HLType>(() => GetOrCreateType(PlatformType.SystemString));
        public static HLType SystemString { get { return sSystemString.Value; } }

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
