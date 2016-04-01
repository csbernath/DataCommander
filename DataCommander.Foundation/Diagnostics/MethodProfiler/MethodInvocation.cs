namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System.Threading;

    internal sealed class MethodInvocation
    {
        private static int idSequence;

        public MethodInvocation(
            MethodInvocation parent,
            int methodId,
            long beginTime)
        {
            this.Parent = parent;
            this.Id = Interlocked.Increment(ref idSequence);
            this.MethodId = methodId;
            this.BeginTime = beginTime;
        }

        public MethodInvocation Parent { get; }

        public int Id { get; }

        public int MethodId { get; }

        public long BeginTime { get; }

        public long EndTime { get; set; }
    }
}