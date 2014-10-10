#if FOUNDATION_3_5

namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class Task : IAsyncResult, IDisposable
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private Action<Object> action;
        private Object state;
        private TaskCreationOptions taskCreationOptions;
        private static Int32 idCounter;
        private Int32 id;
        private readonly EventWaitHandle completedEvent = new EventWaitHandle( false, EventResetMode.ManualReset );
        private Boolean isCompleted;
        private Exception exception;

        /// <summary>
        /// 
        /// </summary>
        protected Task()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        public Task( Action<Object> action, Object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions )
        {
            this.Construct( action, state, cancellationToken, taskCreationOptions );
        }

        #region IAsyncResult Members

        /// <summary>
        /// 
        /// </summary>
        public Object AsyncState
        {
            get
            {
                return this.state;
            }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                return this.completedEvent;
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsCompleted
        {
            get
            {
                return this.isCompleted;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ( (IDisposable)this.completedEvent ).Dispose();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Int32 Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (this.taskCreationOptions.HasFlag( TaskCreationOptions.LongRunning ))
            {
                var thread = new Thread( this.ExecuteAction );
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                Boolean succeeded = ThreadPool.QueueUserWorkItem( this.ExecuteAction );

                if (!succeeded)
                {
                    this.isCompleted = true;
                    this.completedEvent.Set();
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Boolean Wait( TimeSpan timeout )
        {
            Boolean isCompleted = this.isCompleted;

            if (!isCompleted)
            {
                isCompleted = this.completedEvent.WaitOne( timeout );
            }

            return isCompleted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        protected void Construct( Action<Object> action, Object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions )
        {
            Contract.Requires<ArgumentNullException>( action != null );
            this.action = action;
            this.state = state;
            this.taskCreationOptions = taskCreationOptions;

            this.id = Interlocked.Increment( ref idCounter );
        }

        private void ExecuteAction( Object state )
        {
            try
            {
                this.action( this.state );
            }
            catch (Exception e)
            {
                this.exception = e;
            }

            this.isCompleted = true;
            this.completedEvent.Set();
        }
    }
}

#endif