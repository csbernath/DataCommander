namespace DataCommander.Foundation.Threading
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncQueueConsumeEventArgs : EventArgs
    {
        internal AsyncQueueConsumeEventArgs(Object item)
        {
            this.item = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public Object Item
        {
            get
            {
                return this.item;
            }
        }

        private Object item;
    }
}