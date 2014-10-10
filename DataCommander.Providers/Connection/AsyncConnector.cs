namespace DataCommander.Providers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Threading;

    internal sealed class AsyncConnector
    {
        private ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private ConnectionProperties connectionProperties;
        private WorkerThread thread;
        private EndConnectionOpen endConnectionOpen;
        private long duration;

        public AsyncConnector( ConnectionProperties connectionProperties )
        {
            this.connectionProperties = connectionProperties;
        }

        public long Duration
        {
            get
            {
                return this.duration;
            }
        }

        private void Start()
        {
            Exception exception;

            try
            {
                IProvider provider = ProviderFactory.CreateProvider( connectionProperties.providerName );
                connectionProperties.provider = provider;
                ConnectionBase connection = provider.CreateConnection( connectionProperties.connectionString );
                Contract.Assert( connection != null );
                connection.ConnectionName = connectionProperties.connectionName;
                this.duration = Stopwatch.GetTimestamp();
                connection.Open();
                this.duration = Stopwatch.GetTimestamp() - this.duration;
                connectionProperties.connection = connection;
                exception = null;
            }
            catch (Exception e)
            {
                exception = e;
            }

            endConnectionOpen( exception );
        }

        public void BeginOpen( EndConnectionOpen endConnectionOpen )
        {
            Contract.Assert( this.thread == null );
            this.endConnectionOpen = endConnectionOpen;
            this.thread = new WorkerThread( this.Start );
            this.thread.Name = "DataCommander.Providers.AsyncConnector.BeginOpen";
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        public void Cancel()
        {
            ThreadPool.QueueUserWorkItem( CancelWaitCallback );
        }

        private void CancelWaitCallback( object state )
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                thread.Abort();
            }
        }
    }
}