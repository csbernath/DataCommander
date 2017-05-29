namespace DataCommander.Foundation.Threading
{
    a
    using System;
#if FOUNDATION_4_0
    using System.Diagnostics.Contracts;
#endif
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public class CallbackEvent
    {
        private ManualResetEvent manualResetEvent;
        private Int32 count;
        private Int32 current;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manualResetEvent"></param>
        /// <param name="count"></param>
        public CallbackEvent(
            ManualResetEvent manualResetEvent,
            Int32 count )
        {
#if FOUNDATION_4_0
            Contract.Requires(manualResetEvent != null);
            Contract.Requires(count >= 0);
#else
            Assert.IsNotNull( manualResetEvent, "manualResetEvent" );
            Assert.Compare<Int32>( count, Comparers.GreaterThanOrEqual, 0, "count", null );
#endif
            this.manualResetEvent = manualResetEvent;
            this.count = count;
        }

        /// <summary>
        /// 
        /// </summary>
        public ManualResetEvent Event
        {
            get
            {
                return this.manualResetEvent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Callback()
        {
            Interlocked.Increment( ref this.current );

            if (this.current == this.count)
            {
                this.manualResetEvent.Set();
            }
        }
    }
}