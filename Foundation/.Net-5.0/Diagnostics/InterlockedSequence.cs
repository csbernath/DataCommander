using System.Threading;

namespace Foundation.Diagnostics
{
    internal sealed class InterlockedSequence
    {
        private long _value;

        public InterlockedSequence(int value) => _value = value;

        public long Next()
        {
            var next = Interlocked.Increment(ref _value);
            return next;
        }
    }
}