namespace System
{
    public class SystemException : Exception
    {
        public SystemException() : base("A SystemException has occured.") { }

        public SystemException(string pMessage) : base(pMessage) { }

        public SystemException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException) { }
    }
}
