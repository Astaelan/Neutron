using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLStringTable
    {
        private StringBuilder mBuilder = new StringBuilder(32768);
        private Dictionary<string, int> mCache = new Dictionary<string, int>(512);

        public HLStringTable()
        {
            Include("");
        }

        public int Include(string pString)
        {
            int offset = 0;
            if (!mCache.TryGetValue(pString, out offset))
            {
                offset = mBuilder.Length;
                mBuilder.Append(pString);
                mBuilder.Append('\0');
                mCache.Add(pString, offset);
            }
            return offset;
        }

        public byte[] ToASCIIBytes() { return Encoding.ASCII.GetBytes(mBuilder.ToString()); }
    }
}
