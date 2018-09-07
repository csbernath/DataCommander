using System;

namespace Foundation.Diagnostics
{
    public sealed class Progress
    {
        private readonly int _count;
        private readonly int _timeout;
        private int _value;
        private int _reportedTimestamp;

        public Progress(int count, int timeout)
        {
            _count = count;
            _timeout = timeout;
        }

        public event EventHandler<UpdatedEventArgs> Updated;

        public void Increment()
        {
            _value++;

            var timestamp = Environment.TickCount;
            var elapsed = timestamp - _reportedTimestamp;
            if (_timeout < elapsed)
            {
                Updated(this, new UpdatedEventArgs(_value));
                _reportedTimestamp = timestamp;
            }
        }

        public sealed class UpdatedEventArgs : EventArgs
        {
            public UpdatedEventArgs(int value) => Value = value;

            public int Value { get; }
        }
    }
}