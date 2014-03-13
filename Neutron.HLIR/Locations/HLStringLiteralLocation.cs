using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLStringLiteralLocation : HLLocation
    {
        public static HLStringLiteralLocation Create(string pLiteral)
        {
            HLStringLiteralLocation location = new HLStringLiteralLocation(HLDomain.SystemString);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLStringLiteralLocation(HLType pType) : base(pType) { }

        private string mLiteral = null;
        public string Literal { get { return mLiteral; } }

        public override string ToString() { return string.Format("({0})\"{1}\"", Type, mLiteral); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            List<LLLocation> parameters = new List<LLLocation>();

            LLLocation locationStringReference = LLTemporaryLocation.Create(pFunction.CreateTemporary(HLDomain.SystemStringCtorWithCharPointer.Parameters[0].Type.LLType.PointerDepthPlusOne));
            pFunction.CurrentBlock.EmitAllocate(locationStringReference);
            pFunction.CurrentBlock.EmitStore(locationStringReference, LLLiteralLocation.Create(LLLiteral.Create(locationStringReference.Type.PointerDepthMinusOne, "zeroinitializer")));

            parameters.Add(locationStringReference);
            parameters.Add(LLLiteralLocation.Create(LLLiteral.Create(HLDomain.GCRoot.Parameters[1].Type, "null")));
            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.GCRoot), parameters);

            parameters.Clear();
            parameters.Add(locationStringReference);

            char[] data = Literal.ToCharArray();
            Array.Resize(ref data, data.Length + 1);

            LLType typeChar = LLModule.GetOrCreateSignedType(16);
            LLType typeLiteralData = LLModule.GetOrCreateArrayType(typeChar, data.Length);
            LLLocation locationLiteralData = LLTemporaryLocation.Create(pFunction.CreateTemporary(typeLiteralData.PointerDepthPlusOne));
            pFunction.CurrentBlock.EmitAllocate(locationLiteralData);
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int index = 0; index < data.Length; ++index)
            {
                if (index > 0) sb.Append(", ");
                sb.AppendFormat("{0} {1}", typeChar, (int)data[index]);
            }
            sb.Append(" ]");
            pFunction.CurrentBlock.EmitStore(locationLiteralData, LLLiteralLocation.Create(LLLiteral.Create(typeLiteralData, sb.ToString())));
            parameters.Add(locationLiteralData);

            for (int index = 1; index < parameters.Count; ++index)
                parameters[index] = pFunction.CurrentBlock.EmitConversion(parameters[index], HLDomain.SystemStringCtorWithCharPointer.Parameters[index].Type.LLType);

            pFunction.CurrentBlock.EmitCall(null, LLFunctionLocation.Create(HLDomain.SystemStringCtorWithCharPointer.LLFunction), parameters);

            LLLocation locationString = LLTemporaryLocation.Create(pFunction.CreateTemporary(Type.LLType));
            pFunction.CurrentBlock.EmitLoad(locationString, locationStringReference);
            return locationString;
        }
    }
}
