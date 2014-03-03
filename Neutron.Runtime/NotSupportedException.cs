namespace System
{
    public class NotSupportedException : SystemException
    {
        public NotSupportedException() : base("Operation is not supported.") { }

        public NotSupportedException(string pMessage) : base(pMessage) { }

        public NotSupportedException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException) { }
    }
}
