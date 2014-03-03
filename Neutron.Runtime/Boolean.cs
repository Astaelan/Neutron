namespace System
{
	public struct Boolean : IComparable, IComparable<bool>, IEquatable<bool>
    {
        public static readonly string TrueString = "True";
        public static readonly string FalseString = "False";

#pragma warning disable 0649
        private bool mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is bool)) throw new ArgumentException();
            return CompareTo((bool)pObject);
        }
        public int CompareTo(bool pValue) { return mValue == pValue ? 0 : (mValue ? 1 : -1); }

        public override bool Equals(object pObject) { return (pObject is bool) && mValue == (bool)pObject; }
        public bool Equals(bool pValue) { return mValue == pValue; }

        public override int GetHashCode() { return mValue ? 1 : 0; }

        public override string ToString() { return mValue ? TrueString : FalseString; }
    }
}
