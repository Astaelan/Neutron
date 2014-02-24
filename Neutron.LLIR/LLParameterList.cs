using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLParameterList : IEnumerable<LLParameter>
    {
        private List<LLParameter> mList = new List<LLParameter>();
        private Dictionary<string, LLParameter> mDictionary = new Dictionary<string, LLParameter>();

        public LLParameterList() { }

        public LLParameterList(IEnumerable<LLParameter> pParameters)
        {
            foreach (LLParameter parameter in pParameters)
            {
                mList.Add(parameter);
                mDictionary.Add(parameter.Identifier, parameter);
            }
        }

        public IEnumerator<LLParameter> GetEnumerator() { return mList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return mList.GetEnumerator(); }

        public int Count { get { return mList.Count; } }

        public LLParameter Add(LLParameter pParameter)
        {
            mList.Add(pParameter);
            mDictionary.Add(pParameter.Identifier, pParameter);
            return pParameter;
        }

        public LLParameter Remove(LLParameter pParameter)
        {
            mList.Remove(pParameter);
            mDictionary.Remove(pParameter.Identifier);
            return pParameter;
        }

        public bool TryGetValue(string pIdentifier, out LLParameter pParameter) { return mDictionary.TryGetValue(pIdentifier, out pParameter); }

        public LLParameter this[int pIndex] { get { return mList[pIndex]; } }
        public LLParameter this[string pIdentifier] { get { return mDictionary[pIdentifier]; } }

        public List<LLType> ToTypeList() { return mList.ConvertAll(p => p.Type); }
    }
}
