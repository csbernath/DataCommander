#if FOUNDATION_3_5

namespace Foundation.Threading.Tasks
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using Foundation.Diagnostics;
    using Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class Task : IAsyncResult, IDisposable
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private Action<object> action;
        private object state;
        private TaskCreationOptions taskCreationOptions;
        private static int idCounter;
        private int id;
        private readonly EventWaitHandle completedEvent = new EventWaitHandle( false, EventResetMode.ManualReset );
        private bool isCompleted;
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
        public Task( Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions )
        {
            this.Construct( action, state, cancellationToken, taskCreationOptions );
        }

        #region IAsyncResult Members

        /// <summary>
        /// 
        /// </summary>
        public object AsyncState
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
        public bool IsCompleted
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
        public int Id
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
                bool succeeded = ThreadPool.QueueUserWorkItem( this.ExecuteAction );

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
        public bool Wait( TimeSpan timeout )
        {
            bool isCompleted = this.isCompleted;

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
        protected void Construct( Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions )
        {
            FoundationContract.Requires<ArgumentNullException>( action != null );
            this.action = action;
            this.state = state;
            this.taskCreationOptions = taskCreationOptions;

            this.id = Interlocked.Increment( ref idCounter );
        }

        private void ExecuteAction( object state )
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