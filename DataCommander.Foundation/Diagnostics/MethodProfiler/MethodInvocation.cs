using System.Threading;

namespace Foundation.Diagnostics.MethodProfiler
{
    internal sealed class MethodInvocation
    {
        private static int _idSequence;

        public MethodInvocation(
            MethodInvocation parent,
            int methodId,
            long beginTime)
        {
            this.Parent = parent;
            this.Id = Interlocked.Increment(ref _idSequence);
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