namespace System
{
    public abstract class Delegate
    {
        protected object mTargetObj = null;
        protected RuntimeMethodHandle mTargetMethod = new RuntimeMethodHandle();
        protected Delegate mNext = null;

        protected Delegate() { }
    }
}
