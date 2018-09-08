using System.Threading;

namespace Foundation
{
    public sealed class ReferenceCounter
    {
        private int _value;

        public void Add()
        {
            Interlocked.Increment(ref _value);
        }

        public void Remove()
        {
            Interlocked.Decrement(ref _value);
        }

        public int Value => _value;
    }
}