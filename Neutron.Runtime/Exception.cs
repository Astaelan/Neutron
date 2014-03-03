namespace System
{
    public class Exception
    {
        private string mMessage;
        private Exception mInnerException;

        public Exception() { }

        public Exception(string pMessage) { mMessage = pMessage; }

        public Exception(string pMessage, Exception pInnerException)
        {
            mMessage = pMessage;
            mInnerException = pInnerException;
        }

        public virtual string Message { get { return mMessage; } }

        public Exception InnerException { get { return mInnerException; } }
    }
}
