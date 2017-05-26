#if FOUNDATION_3_5

namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public class CancellationTokenSource : IDisposable
    {
        internal static readonly CancellationTokenSource NotCancelable = new CancellationTokenSource( false );
        private bool isCancellationRequested;
        private ManualResetEvent kernelEvent;

        private CancellationTokenSource( bool set )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CancellationTokenSource()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                return this.isCancellationRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken Token
        {
            get
            {
                return new CancellationToken( this );
            }
        }

        internal WaitHandle WaitHandle
        {
            get
            {
                if (this.kernelEvent == null)
                {
                    var manualResetEvent = new ManualResetEvent( false );
                    if (Interlocked.CompareExchange<ManualResetEvent>( ref this.kernelEvent, manualResetEvent, null ) != null)
                    {
                        ( (IDisposable)manualResetEvent ).Dispose();
                    }
                }

                if (this.isCancellationRequested)
                {
                    this.kernelEvent.Set();
                }

                return this.kernelEvent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}

#endif