using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLFieldList : IEnumerable<HLField>
    {
        private List<HLField> mList = new List<HLField>();
        private Dictionary<string, HLField> mDictionary = new Dictionary<string, HLField>();

        public HLFieldList() { }

        public HLFieldList(IEnumerable<HLField> pFields)
        {
            foreach (HLField field in pFields)
            {
                mList.Add(field);
                mDictionary.Add(field.Name, field);
            }
        }

        public IEnumerator<HLField> GetEnumerator() { return mList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return mList.GetEnumerator(); }

        public int Count { get { return mList.Count; } }

        public HLField Add(HLField pField)
        {
            mList.Add(pField);
            mDictionary.Add(pField.Name, pField);
            return pField;
        }

        public HLField Remove(HLField pField)
        {
            mList.Remove(pField);
            mDictionary.Remove(pField.Name);
            return pField;
        }

        public bool TryGetValue(string pIdentifier, out HLField pField) { return mDictionary.TryGetValue(pIdentifier, out pField); }

        public HLField this[int pIndex] { get { return mList[pIndex]; } }
        public HLField this[string pIdentifier] { get { return mDictionary[pIdentifier]; } }

        public List<HLType> ToTypeList() { return mList.ConvertAll(p => p.Type); }
    }
}
