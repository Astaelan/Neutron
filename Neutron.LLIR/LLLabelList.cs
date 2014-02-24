using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLLabelList : IEnumerable<LLLabel>
    {
        private List<LLLabel> mList = new List<LLLabel>();
        private Dictionary<int, LLLabel> mDictionary = new Dictionary<int, LLLabel>();

        public LLLabelList() { }

        public LLLabelList(IEnumerable<LLLabel> pLabels)
        {
            foreach (LLLabel label in pLabels)
            {
                mList.Add(label);
                mDictionary.Add(label.Identifier, label);
            }
        }

        public IEnumerator<LLLabel> GetEnumerator() { return mList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return mList.GetEnumerator(); }

        public int Count { get { return mList.Count; } }

        public LLLabel Add(LLLabel pLabel)
        {
            mList.Add(pLabel);
            mDictionary.Add(pLabel.Identifier, pLabel);
            return pLabel;
        }

        public LLLabel Remove(LLLabel pLabel)
        {
            mList.Remove(pLabel);
            mDictionary.Remove(pLabel.Identifier);
            return pLabel;
        }

        public LLLabel GetByIdentifier(int pIdentifier) { return mDictionary[pIdentifier]; }
        public bool TryGetValue(int pIdentifier, out LLLabel pLabel) { return mDictionary.TryGetValue(pIdentifier, out pLabel); }

        public LLLabel this[int pIndex] { get { return mList[pIndex]; } }
    }
}
