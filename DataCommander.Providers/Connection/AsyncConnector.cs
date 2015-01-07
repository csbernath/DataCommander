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
        private readonly ConnectionProperties connectionProperties;
        private WorkerThread thread;
        private Action<Exception> endConnectionOpen;
        private long duration;

        public AsyncConnector(ConnectionProperties connectionProperties)
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
                if (connectionProperties.Provider == null)
                {
                    IProvider provider = ProviderFactory.CreateProvider(connectionProperties.ProviderName);
                    connectionProperties.Provider = provider;
                }
                ConnectionBase connection = connectionProperties.Provider.CreateConnection(connectionProperties.ConnectionString);
                Contract.Assert(connection != null);
                connection.ConnectionName = connectionProperties.ConnectionName;
                this.duration = Stopwatch.GetTimestamp();
                connection.Open();
                this.duration = Stopwatch.GetTimestamp() - this.duration;
                connectionProperties.Connection = connection;
                exception = null;
            }
            catch (Exception e)
            {
                exception = e;
            }

            endConnectionOpen(exception);
        }

        public void BeginOpen(Action<Exception> endConnectionOpen)
        {
            Contract.Assert(this.thread == null);
            this.endConnectionOpen = endConnectionOpen;
            this.thread = new WorkerThread(this.Start) {Name = "DataCommander.Providers.AsyncConnector.BeginOpen", IsBackground = true};
            this.thread.Start();
        }

        public void Cancel()
        {
            ThreadPool.QueueUserWorkItem(CancelWaitCallback);
        }

        private void CancelWaitCallback(object state)
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                thread.Abort();
            }
        }
    }
}