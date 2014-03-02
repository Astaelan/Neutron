namespace System
{
    public class Exception
    {
        private Exception mInnerException;
        private string mMessage;

        public Exception() { }

        public Exception(string message) { mMessage = message; }

        public Exception(string message, Exception innerException)
        {
            mInnerException = innerException;
            mMessage = message;
        }

        public virtual string Message { get { return mMessage; } }

        public Exception InnerException { get { return mInnerException; } }
    }
}
