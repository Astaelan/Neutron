namespace System
{
    public class ArgumentNullException : ArgumentException
    {
        public ArgumentNullException() : base("Argument cannot be null.") { }

        public ArgumentNullException(string pParamName) : base("Argument cannot be null.", pParamName) { }

        public ArgumentNullException(string pParamName, string pMessage) : base(pMessage, pParamName) { }

        public ArgumentNullException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException) { }
    }
}
