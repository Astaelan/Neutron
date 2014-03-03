namespace System
{
    public class InvalidOperationException : SystemException
    {
        public InvalidOperationException() : base("Operation is not valid due to the current state of the object") { }

        public InvalidOperationException(string pMessage) : base(pMessage) { }

        public InvalidOperationException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException) { }
    }
}
