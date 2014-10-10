namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Threading;

    internal sealed class MethodInvocation
    {
        private static Int32 idSequence;
        private readonly MethodInvocation parent;
        private readonly Int32 id;
        private readonly Int32 methodId;
        private Int64 beginTime;
        private Int64 endTime;

        public MethodInvocation(
            MethodInvocation parent,
            Int32 methodId,
            Int64 beginTime)
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

        public Int32 Id
        {
            get
            {
                return this.id;
            }
        }

        public Int32 MethodId
        {
            get
            {
                return this.methodId;
            }
        }

        public Int64 BeginTime
        {
            get
            {
                return this.beginTime;
            }
        }

        public Int64 EndTime
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