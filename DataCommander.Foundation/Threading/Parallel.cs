#if FOUNDATION_3_5

namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public static class Parallel
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actions"></param>
        public static void Invoke( params Action[] actions )
        {
            var invoker = new Invoker( actions );
            invoker.Invoke();

            if (!invoker.IsCompleted)
            {
                WaitHandle waitHandle = invoker.AsyncWaitHandle;
                waitHandle.WaitOne();
                waitHandle.Close();
            }
        }

        #endregion

        #region Private Classes

        private sealed class Invoker
        {
            private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
            private Action[] actions;
            private Int32 callbackCount;
            private EventWaitHandle waitHandle;
            private bool isCompleted;
            private bool completedSynchronously;

            public Invoker( Action[] actions )
            {
                Contract.Requires<ArgumentNullException>( actions != null );
                Contract.Requires<ArgumentNullException>( Contract.ForAll( actions, action => action != null ) );
                this.actions = actions;
            }

            public void Invoke()
            {
                if (this.actions.Length > 0)
                {
                    this.waitHandle = new EventWaitHandle( false, EventResetMode.ManualReset );

                    for (Int32 i = 0; i < this.actions.Length; i++)
                    {
                        Action action = this.actions[ i ];
                        Contract.Assert( action != null, String.Format( "action[{0}]", i ) );
                        bool succeeded = ThreadPool.QueueUserWorkItem( this.Callback, action );
                        Contract.Assert( succeeded, "Adding work item to thread pool failed." );
                    }
                }
                else
                {
                    this.completedSynchronously = true;
                    this.Complete();
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return this.isCompleted;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return this.completedSynchronously;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return this.waitHandle;
                }
            }

            private void Complete()
            {
                this.isCompleted = true;

                if (this.waitHandle != null)
                {
                    this.waitHandle.Set();
                }
            }

            private void Callback( Object state )
            {
                var action = (Action)state;

                try
                {
                    action();
                }
                catch (Exception e)
                {
                    log.Error( e.ToLogString() );
                }

                Interlocked.Increment( ref this.callbackCount );

                if (this.callbackCount == this.actions.Length)
                {
                    this.completedSynchronously = false;
                    this.Complete();
                }
            }
        }

        #endregion
    }
}

#endif