using System;

namespace Foundation.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncQueueConsumeEventArgs : EventArgs
    {
        internal AsyncQueueConsumeEventArgs(object item)
        {
            Item = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Item { get; }
    }
}