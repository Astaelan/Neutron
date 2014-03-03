namespace System
{
	public struct Char : IComparable, IComparable<char>, IEquatable<char>
    {
#pragma warning disable 0649
        private char mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is char)) throw new ArgumentException();
            return CompareTo((char)pObject);
        }
        public int CompareTo(char pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is char) && mValue == (char)pObject; }
        public bool Equals(char pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        public override string ToString() { return new string(mValue, 1); }
    }
}
