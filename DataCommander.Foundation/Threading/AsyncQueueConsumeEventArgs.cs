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
            this.item = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Item => this.item;

        private readonly object item;
    }
}