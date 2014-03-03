namespace System
{
    public class ArgumentException : SystemException
    {
		private string mParamName;

        public ArgumentException() : base("An invalid argument was specified.") { }

        public ArgumentException(string pMessage) : base(pMessage) { }

        public ArgumentException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException) { }

        public ArgumentException(string pMessage, string pParamName) : base(pMessage) { mParamName = pParamName; }

        public virtual string ParamName { get { return mParamName; } }

        public override string Message
        {
            get
            {
                string baseMessage = base.Message;
                if (baseMessage == null) baseMessage = "An invalid argument was specified.";
                if (mParamName == null) return baseMessage;
                return baseMessage + Environment.NewLine + "Parameter name: " + mParamName;
            }
        }
    }
}
