namespace DataCommander.Foundation.Threading
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncQueueConsumeEventArgs : EventArgs
    {
        internal AsyncQueueConsumeEventArgs(object item)
        {
            this.Item = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Item { get; }
    }
}