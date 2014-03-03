namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public sealed class DefaultMemberAttribute : Attribute
    {
        private string mMemberName;

        public DefaultMemberAttribute(string pMemberName) { mMemberName = pMemberName; }

        public string MemberName { get { return mMemberName; } }
    }
}
