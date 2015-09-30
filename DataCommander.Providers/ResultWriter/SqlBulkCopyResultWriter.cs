namespace DataCommander.Providers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;

    internal sealed class SqlBulkCopyResultWriter : IResultWriter
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IResultWriter logResultWriter;
        private readonly Action<InfoMessage> addInfoMessage;
        private IProvider destinationProvider;
        private readonly ConnectionBase destinationConnection;
        private readonly SqlConnection destinationSqlConnection;
        private readonly string tableName;
        private readonly Action<IDbTransaction> setTransaction;
        private readonly CancellationToken cancellationToken;
        private IDbTransaction transaction;
        //private IDbCommand insertCommand;
        private Converter<object, object>[] converters = null;
        //private IDbDataParameter[] parameters;
        private ConcurrentQueue<QueueItem> queue;
        private Task task;
        private EventWaitHandle enqueueEvent;
        private bool writeEnded;
        private bool canConvertCommandToString;
        private long readRowCount;
        private long insertedRowCount;
        private SqlBulkCopy sqlBulkCopy;

        public SqlBulkCopyResultWriter(
            Action<InfoMessage> addInfoMessage,
            IProvider destinationProvider,
            ConnectionBase destinationConnection,
            string tableName,
            Action<IDbTransaction> setTransaction,
            CancellationToken cancellationToken)
        {
            Contract.Requires(destinationProvider.DbProviderFactory == SqlClientFactory.Instance);
            this.destinationSqlConnection = (SqlConnection)destinationConnection.Connection;

            this.logResultWriter = new LogResultWriter(addInfoMessage);
            this.addInfoMessage = addInfoMessage;
            this.destinationProvider = destinationProvider;
            this.canConvertCommandToString = destinationProvider.CanConvertCommandToString;
            this.destinationConnection = destinationConnection;
            this.tableName = tableName;
            this.setTransaction = setTransaction;
            this.cancellationToken = cancellationToken;
        }

        #region IResultWriter Members

        void IResultWriter.Begin()
        {
            this.logResultWriter.Begin();
        }

        void IResultWriter.BeforeExecuteReader(IProvider provider, IDbCommand command)
        {
            this.logResultWriter.BeforeExecuteReader(provider, command);
        }

        void IResultWriter.AfterExecuteReader()
        {
            this.logResultWriter.AfterExecuteReader();
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            this.logResultWriter.AfterCloseReader(affectedRows);
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.logResultWriter.WriteTableBegin(schemaTable);
            if (this.transaction == null)
            {
                this.transaction = this.destinationConnection.Connection.BeginTransaction();
                this.setTransaction(this.transaction);
            }

            var sqlTransaction = (SqlTransaction)this.transaction;
            this.sqlBulkCopy = new SqlBulkCopy(this.destinationSqlConnection, SqlBulkCopyOptions.Default, sqlTransaction);
            this.sqlBulkCopy.BulkCopyTimeout = int.MaxValue;
            this.sqlBulkCopy.DestinationTableName = this.tableName;
            this.sqlBulkCopy.NotifyAfter = 10000;
            this.sqlBulkCopy.SqlRowsCopied += this.sqlBulkCopy_SqlRowsCopied;
        }

        private void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            string message = string.Format("{0} rows copied to destination.", e.RowsCopied);
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));

            if (this.cancellationToken.IsCancellationRequested)
            {
                e.Abort = true;
            }
        }

        private void InsertItems(IEnumerable<QueueItem> items)
        {
            foreach (var item in items)
            {
                var rows = item.Rows;
                var dataTable = new DataTable();

                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    var row = rows[rowIndex];
                    var dataRow = dataTable.NewRow();
                    for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        object sourceValue = row[columnIndex];
                        Converter<object, object> converter = this.converters[columnIndex];
                        object destinationValue;

                        if (converter != null)
                        {
                            destinationValue = converter(sourceValue);
                        }
                        else
                        {
                            destinationValue = sourceValue;
                        }

                        dataRow[columnIndex] = destinationValue;
                    }

                    dataTable.Rows.Add(dataRow);
                    this.insertedRowCount++;
                }

                this.sqlBulkCopy.WriteToServer(dataTable);
            }

            string message = string.Format("{0} rows inserted.", this.insertedRowCount);
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        private void Dequeue()
        {
            try
            {
                using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
                {
                    while (true)
                    {
                        methodLog.Write(LogLevel.Trace, "this.queue.Count: {0}", this.queue.Count);
                        if (this.queue.Count > 0)
                        {
                            var items = new List<QueueItem>(this.queue.Count);
                            while (true)
                            {
                                QueueItem item;
                                bool succeeded = this.queue.TryDequeue(out item);
                                if (succeeded)
                                {
                                    items.Add(item);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            this.InsertItems(items);
                        }

                        if (this.queue.Count == 0)
                        {
                            methodLog.Write(LogLevel.Trace, "this.writeEnded: {0}", this.writeEnded);
                            if (this.writeEnded)
                            {
                                break;
                            }
                            else
                            {
                                methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 );...");
                                this.enqueueEvent.WaitOne(1000);
                                methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 ); finished.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void IResultWriter.FirstRowReadBegin()
        {
            this.logResultWriter.FirstRowReadBegin();
            if (this.queue == null)
            {
                this.queue = new ConcurrentQueue<QueueItem>();
                this.enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
                this.task = new Task(this.Dequeue, TaskCreationOptions.LongRunning);
                this.task.Start();
            }
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            this.logResultWriter.FirstRowReadEnd(dataTypeNames);
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            this.logResultWriter.WriteRows(rows, rowCount);
            this.readRowCount += rowCount;
            string message = string.Format("{0} row(s) read.", this.readRowCount);
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            object[][] targetRows = new object[rowCount][];
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                object[] sourceRow = rows[rowIndex];
                int columnCount = sourceRow.Length;
                var targetRow = new object[columnCount];
                Array.Copy(sourceRow, targetRow, columnCount);
                targetRows[rowIndex] = targetRow;
            }

            var queueItem = new QueueItem
            {
                Rows = targetRows
            };

            this.queue.Enqueue(queueItem);
            this.enqueueEvent.Set();
            
            while (!this.cancellationToken.IsCancellationRequested && this.queue.Count > 10)
            {
                this.cancellationToken.WaitHandle.WaitOne(500);
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            this.logResultWriter.WriteTableEnd();
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            this.logResultWriter.End();
            using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
            {
                this.writeEnded = true;
                if (this.task != null && !this.task.IsCompleted)
                {
                    methodLog.Write(LogLevel.Trace, "Waiting 500 ms for task...");
                    this.task.Wait(500);
                }
            }
        }

        #endregion

        private sealed class QueueItem
        {
            public object[][] Rows;
        }
    }
}