namespace System
{
    public class ArgumentOutOfRangeException : ArgumentException
    {
        public ArgumentOutOfRangeException() : base("Argument is out of range.") { }

        public ArgumentOutOfRangeException(string pParamName) : base("Argument is out of range.", pParamName) { }

        public ArgumentOutOfRangeException(string pParamName, string pMessage) : base(pMessage, pParamName) { }
    }
}
