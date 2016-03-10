namespace DataCommander.Foundation.Diagnostics
{
    using System.Threading;

    internal sealed class MethodInvocation
    {
        private static int idSequence;
        private readonly MethodInvocation parent;
        private readonly int id;
        private readonly int methodId;
        private readonly long beginTime;
        private long endTime;

        public MethodInvocation(
            MethodInvocation parent,
            int methodId,
            long beginTime)
        {
            this.parent = parent;
            this.id = Interlocked.Increment(ref idSequence);
            this.methodId = methodId;
            this.beginTime = beginTime;
        }

        public MethodInvocation Parent => this.parent;

        public int Id => this.id;

        public int MethodId => this.methodId;

        public long BeginTime => this.beginTime;

        public long EndTime
        {
            get
            {
                return this.endTime;
            }

            set
            {
                this.endTime = value;
            }
        }
    }
}