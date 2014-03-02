namespace System
{
    public abstract class Delegate
    {
        private object mTargetObj = null;
        private IntPtr mTargetMethod = IntPtr.Zero;
        protected Delegate mNext = null;

        protected Delegate() { }
    }
}
