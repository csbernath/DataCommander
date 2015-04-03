namespace DataCommander.Foundation.Threading
{
    using System;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// This class is a specialized <see cref="WorkerThread"/> which
    /// repeats an operation until the operation is stopped.
    /// E.g Windows Services can use this class.
    /// </summary>
    /// <remarks>
    /// There are two ways to use this class:
    /// <list type="table">
    /// <item>
    ///        <term>With inheritance</term>
    ///        <description>Call ctor</description>
    ///    </item>
    /// <item>
    ///        <term>Without inheritance (as a member field)</term>
    ///        <description>Call <see cref="Initialize"/></description>
    ///    </item>
    /// </list>
    /// </remarks>
    public class LoopThread
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private ILoopable loopable;
        private WorkerThread thread;

        /// <summary>
        /// Inherited class must call this constructor.
        /// </summary>
        protected LoopThread()
        {
        }

        /// <summary>
        /// If this class is not inherited the caller can initialize the instance here.
        /// </summary>
        /// <param name="loopable"></param>
        public LoopThread( ILoopable loopable )
        {
            this.Initialize( loopable );
        }

        /// <summary>
        /// Gets the underlying <see cref="WorkerThread"/>.
        /// </summary>
        public WorkerThread Thread
        {
            get
            {
                return this.thread;
            }
        }

        /// <summary>
        /// Inherited class must initialize the base class with this method.
        /// </summary>
        /// <param name="loopable"></param>
        protected void Initialize( ILoopable loopable )
        {
            this.loopable = loopable;
            this.thread = new WorkerThread( this.Start );
        }

        private void Start()
        {
            Exception exception = null;

            while (!this.thread.IsStopRequested)
            {
                try
                {
                    if (!this.thread.IsStopRequested)
                    {
                        this.loopable.First( exception );
                        exception = null;

                        while (!this.thread.IsStopRequested)
                        {
                            this.loopable.Next();
                        }
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                    log.Write( LogLevel.Error, "LoopThread({0},{1}) exception:\r\n{2}", this.thread.Name, this.thread.ManagedThreadId, e.ToLogString() );
                }
            }

            try
            {
                this.loopable.Last();
            }
            catch (Exception e)
            {
                log.Write( LogLevel.Error, "LoopThread({0},{1}) exception:\r\n{2}", this.thread.Name, this.thread.ManagedThreadId, e.ToLogString() );
            }
        }
    }
}