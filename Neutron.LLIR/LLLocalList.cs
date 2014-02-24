using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLLocalList : IEnumerable<LLLocal>
    {
        private List<LLLocal> mList = new List<LLLocal>();
        private Dictionary<string, LLLocal> mDictionary = new Dictionary<string, LLLocal>();

        public LLLocalList() { }

        public LLLocalList(IEnumerable<LLLocal> pLocals)
        {
            foreach (LLLocal local in pLocals)
            {
                mList.Add(local);
                mDictionary.Add(local.Identifier, local);
            }
        }

        public IEnumerator<LLLocal> GetEnumerator() { return mList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return mList.GetEnumerator(); }

        public int Count { get { return mList.Count; } }

        public LLLocal Add(LLLocal pLocal)
        {
            mList.Add(pLocal);
            mDictionary.Add(pLocal.Identifier, pLocal);
            return pLocal;
        }

        public LLLocal Remove(LLLocal pLocal)
        {
            mList.Remove(pLocal);
            mDictionary.Remove(pLocal.Identifier);
            return pLocal;
        }

        public bool TryGetValue(string pIdentifier, out LLLocal pLocal) { return mDictionary.TryGetValue(pIdentifier, out pLocal); }

        public LLLocal this[int pIndex] { get { return mList[pIndex]; } }
        public LLLocal this[string pIdentifier] { get { return mDictionary[pIdentifier]; } }
    }
}
