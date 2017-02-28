using System;

namespace DataCommander.Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Progress
    {
        private readonly int _count;
        private readonly int _timeout;
        private int _value;
        private int _reportedTimestamp;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="timeout"></param>
        public Progress(int count, int timeout)
        {
            _count = count;
            _timeout = timeout;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<UpdatedEventArgs> Updated;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public sealed class UpdatedEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            public UpdatedEventArgs(int value)
            {
                Value = value;
            }

            /// <summary>
            /// 
            /// </summary>
            public int Value { get; }
        }
    }
}