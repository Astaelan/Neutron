namespace System
{
	public struct Boolean : IComparable, IComparable<bool>, IEquatable<bool>
    {
        public static readonly string TrueString = "True";
        public static readonly string FalseString = "False";

#pragma warning disable 0649
        private bool mValue;
#pragma warning restore 0649

        //public override string ToString() { return mValue ? TrueString : FalseString; }
    }
}
