﻿#if FOUNDATION_3_5

namespace Foundation.Threading.Tasks
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public struct CancellationToken
    {
        private CancellationTokenSource source;

        internal CancellationToken( CancellationTokenSource source )
        {
            Assert.IsNotNull( source != null );
            this.source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        public static CancellationToken None
        {
            get
            {
                return default( CancellationToken );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                return this.source != null && this.source.IsCancellationRequested;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WaitHandle WaitHandle
        {
            get
            {
                if (this.source == null)
                {
                    this.source = CancellationTokenSource.NotCancelable;
                }

                return this.source.WaitHandle;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            if (this.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}

#endif