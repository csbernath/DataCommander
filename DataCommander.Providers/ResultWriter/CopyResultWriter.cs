namespace DataCommander.Providers.ResultWriter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Connection;
    using Foundation;
    using Foundation.Data;
    using Foundation.Diagnostics;
    using Foundation.Linq;

    internal sealed class CopyResultWriter : IResultWriter
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IResultWriter logResultWriter;
        private readonly Action<InfoMessage> addInfoMessage;
        private readonly IProvider destinationProvider;
        private readonly ConnectionBase destinationConnection;
        private readonly string tableName;
        private readonly Action<IDbTransaction> setTransaction;
        private readonly CancellationToken cancellationToken;
        private IDbTransaction transaction;
        private IDbCommand insertCommand;
        private Converter<object, object>[] converters;
        private IDbDataParameter[] parameters;
        private ConcurrentQueue<QueueItem> queue;
        private Task task;
        private EventWaitHandle enqueueEvent;
        private bool writeEnded;
        private readonly bool canConvertCommandToString;
        private long readRowCount;
        private long insertedRowCount;
        private long waitMilliseconds;

        public CopyResultWriter(
            Action<InfoMessage> addInfoMessage,
            IProvider destinationProvider,
            ConnectionBase destinationConnection,
            string tableName,
            Action<IDbTransaction> setTransaction,
            CancellationToken cancellationToken)
        {
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

        void IResultWriter.Begin(IProvider provider)
        {
            this.logResultWriter.Begin(provider);
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
            this.logResultWriter.BeforeExecuteReader(command);
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            this.logResultWriter.AfterExecuteReader(fieldCount);
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            this.logResultWriter.AfterCloseReader(affectedRows);
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.logResultWriter.WriteTableBegin(schemaTable);
            this.destinationProvider.CreateInsertCommand(schemaTable, null, this.destinationConnection.Connection, this.tableName, out this.insertCommand,
                out this.converters);
            //  TODO this.messageWriter.WriteLine( this.insertCommand.CommandText );
            this.parameters = this.insertCommand.Parameters.Cast<IDbDataParameter>().ToArray();
            if (this.transaction == null)
            {
                this.transaction = this.destinationConnection.Connection.BeginTransaction();
                this.setTransaction(this.transaction);
            }
            this.insertCommand.Transaction = this.transaction;
        }

        private void InsertItems(IEnumerable<QueueItem> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var rows = item.Rows;
                for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    var row = rows[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        var sourceValue = row[columnIndex];
                        var converter = this.converters[columnIndex];
                        object destinationValue;

                        if (converter != null)
                        {
                            destinationValue = converter(sourceValue);
                        }
                        else
                        {
                            destinationValue = sourceValue;
                        }

                        this.parameters[columnIndex].Value = destinationValue;
                    }

                    if (this.canConvertCommandToString)
                    {
                        if (sb.Length > 0)
                        {
                            sb.AppendLine();
                        }

                        var commandText = this.destinationProvider.CommandToString(this.insertCommand);
                        sb.Append(commandText);
                    }
                    else
                    {
                        this.insertCommand.ExecuteNonQuery();
                    }

                    this.insertedRowCount++;
                }
            }

            if (sb.Length > 0)
            {
                var stopwatch = new Stopwatch();
                var commandText = sb.ToString();
                try
                {
                    var transactionScope = new DbTransactionScope(this.destinationConnection.Connection, this.insertCommand.Transaction);
                    transactionScope.ExecuteNonQuery(new CommandDefinition
                    {
                        CommandText = commandText,
                        CommandTimeout = 3600
                    });
                }
                catch (Exception e)
                {
                    log.Write(LogLevel.Error, "CommandText:\r\n{0}\r\nException:{1}", commandText, e.ToLogString());
                    throw;
                }
            }

            var message =
                $"{this.readRowCount},{this.insertedRowCount},{this.readRowCount - this.insertedRowCount},{this.waitMilliseconds} (rows read,inserted,queued,wait).";

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
                                var succeeded = this.queue.TryDequeue(out item);
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

            var message = $"{this.readRowCount},{this.insertedRowCount},{this.readRowCount - this.insertedRowCount} (rows read,inserted,queued).";

            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            var targetRows = new object[rowCount][];
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var sourceRow = rows[rowIndex];
                var columnCount = sourceRow.Length;
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

            while (!this.cancellationToken.IsCancellationRequested && this.queue.Count > 5)
            {
                this.waitMilliseconds += 500;
                log.Write(LogLevel.Trace, "this.waitMilliseconds: {0}", this.waitMilliseconds);

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