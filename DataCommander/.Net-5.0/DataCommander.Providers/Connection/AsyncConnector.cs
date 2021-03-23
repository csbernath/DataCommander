using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using DataCommander.Providers.Connection;
using DataCommander.Providers2.Connection;
using Foundation.Diagnostics;
using Foundation.Log;
using Foundation.Threading;

namespace DataCommander.Providers
{
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
                if (this.connectionProperties.Provider == null)
                {
                    IProvider provider = ProviderFactory.CreateProvider(this.connectionProperties.ProviderName);
                    this.connectionProperties.Provider = provider;
                }
                ConnectionBase connection = this.connectionProperties.Provider.CreateConnection(this.connectionProperties.ConnectionString);
                Contract.Assert(connection != null);
                connection.ConnectionName = this.connectionProperties.ConnectionName;
                this.duration = Stopwatch.GetTimestamp();
                connection.Open();
                this.duration = Stopwatch.GetTimestamp() - this.duration;
                this.connectionProperties.Connection = connection;
                exception = null;
            }
            catch (Exception e)
            {
                exception = e;
            }

            this.endConnectionOpen(exception);
        }

        public void BeginOpen(Action<Exception> endConnectionOpen)
        {
            Contract.Assert(this.thread == null);
            this.endConnectionOpen = endConnectionOpen;
            this.thread = new WorkerThread(this.Start) { Name = "DataCommander.Providers.AsyncConnector.BeginOpen", IsBackground = true };
            this.thread.Start();
        }

        public void Cancel()
        {
            ThreadPool.QueueUserWorkItem(this.CancelWaitCallback);
        }

        private void CancelWaitCallback(object state)
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                this.thread.Abort();
            }
        }
    }
}