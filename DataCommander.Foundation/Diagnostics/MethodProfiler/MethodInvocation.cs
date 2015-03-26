namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Threading;

    internal sealed class MethodInvocation
    {
        private static int idSequence;
        private readonly MethodInvocation parent;
        private readonly int id;
        private readonly int methodId;
        private long beginTime;
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

        public MethodInvocation Parent
        {
            get
            {
                return this.parent;
            }
        }

        public int Id
        {
            get
            {
                return this.id;
            }
        }

        public int MethodId
        {
            get
            {
                return this.methodId;
            }
        }

        public long BeginTime
        {
            get
            {
                return this.beginTime;
            }
        }

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