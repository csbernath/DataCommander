using Foundation.Data;
using Foundation.Diagnostics.Contracts;
using Foundation.Log;

namespace DataCommander.Providers.ResultWriter
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Connection;
    using Foundation;

    internal sealed class SqlBulkCopyResultWriter : IResultWriter
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IResultWriter _logResultWriter;
        private readonly Action<InfoMessage> _addInfoMessage;
        private IProvider _destinationProvider;
        private readonly ConnectionBase _destinationConnection;
        private readonly SqlConnection _destinationSqlConnection;
        private readonly string _tableName;
        private readonly Action<IDbTransaction> _setTransaction;
        private readonly CancellationToken _cancellationToken;

        private IDbTransaction _transaction;

        //private IDbCommand insertCommand;
        private Converter<object, object>[] _converters = null;

        //private IDbDataParameter[] parameters;
        private ConcurrentQueue<QueueItem> _queue;
        private Task _task;
        private EventWaitHandle _enqueueEvent;
        private bool _writeEnded;
        private bool _canConvertCommandToString;
        private long _readRowCount;
        private long _insertedRowCount;
        private SqlBulkCopy _sqlBulkCopy;

        public SqlBulkCopyResultWriter(
            Action<InfoMessage> addInfoMessage,
            IProvider destinationProvider,
            ConnectionBase destinationConnection,
            string tableName,
            Action<IDbTransaction> setTransaction,
            CancellationToken cancellationToken)
        {
            FoundationContract.Requires<ArgumentException>(destinationProvider.DbProviderFactory == SqlClientFactory.Instance);

            _destinationSqlConnection = (SqlConnection) destinationConnection.Connection;

            _logResultWriter = new LogResultWriter(addInfoMessage);
            _addInfoMessage = addInfoMessage;
            _destinationProvider = destinationProvider;
            _canConvertCommandToString = destinationProvider.CanConvertCommandToString;
            _destinationConnection = destinationConnection;
            _tableName = tableName;
            _setTransaction = setTransaction;
            _cancellationToken = cancellationToken;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider) => _logResultWriter.Begin(provider);
        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command) => _logResultWriter.BeforeExecuteReader(command);
        void IResultWriter.AfterExecuteReader(int fieldCount) => _logResultWriter.AfterExecuteReader(fieldCount);
        void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _logResultWriter.WriteTableBegin(schemaTable);
            if (_transaction == null)
            {
                _transaction = _destinationConnection.Connection.BeginTransaction();
                _setTransaction(_transaction);
            }

            var sqlTransaction = (SqlTransaction) _transaction;
            _sqlBulkCopy = new SqlBulkCopy(_destinationSqlConnection, SqlBulkCopyOptions.Default, sqlTransaction);
            _sqlBulkCopy.BulkCopyTimeout = int.MaxValue;
            _sqlBulkCopy.DestinationTableName = _tableName;
            _sqlBulkCopy.NotifyAfter = 10000;
            _sqlBulkCopy.SqlRowsCopied += sqlBulkCopy_SqlRowsCopied;
        }

        private void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            var message = $"{e.RowsCopied} rows copied to destination.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));

            if (_cancellationToken.IsCancellationRequested)
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

                for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    var row = rows[rowIndex];
                    var dataRow = dataTable.NewRow();
                    for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                    {
                        var sourceValue = row[columnIndex];
                        var converter = _converters[columnIndex];
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
                    _insertedRowCount++;
                }

                _sqlBulkCopy.WriteToServer(dataTable);
            }

            var message = $"{_insertedRowCount} rows inserted.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
        }

        private void Dequeue()
        {
            try
            {
                using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
                {
                    while (true)
                    {
                        methodLog.Write(LogLevel.Trace, "this.queue.Count: {0}", _queue.Count);
                        if (_queue.Count > 0)
                        {
                            var items = new List<QueueItem>(_queue.Count);
                            while (true)
                            {
                                QueueItem item;
                                var succeeded = _queue.TryDequeue(out item);
                                if (succeeded)
                                {
                                    items.Add(item);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            InsertItems(items);
                        }

                        if (_queue.Count == 0)
                        {
                            methodLog.Write(LogLevel.Trace, "this.writeEnded: {0}", _writeEnded);
                            if (_writeEnded)
                            {
                                break;
                            }
                            else
                            {
                                methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 );...");
                                _enqueueEvent.WaitOne(1000);
                                methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 ); finished.");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void IResultWriter.FirstRowReadBegin()
        {
            _logResultWriter.FirstRowReadBegin();
            if (_queue == null)
            {
                _queue = new ConcurrentQueue<QueueItem>();
                _enqueueEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
                _task = new Task(Dequeue, TaskCreationOptions.LongRunning);
                _task.Start();
            }
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            _logResultWriter.FirstRowReadEnd(dataTypeNames);
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            _logResultWriter.WriteRows(rows, rowCount);
            _readRowCount += rowCount;
            var message = $"{_readRowCount} row(s) read.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, null, message));
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

            _queue.Enqueue(queueItem);
            _enqueueEvent.Set();

            while (!_cancellationToken.IsCancellationRequested && _queue.Count > 10)
            {
                _cancellationToken.WaitHandle.WaitOne(500);
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            _logResultWriter.WriteTableEnd();
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            _logResultWriter.End();
            using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
            {
                _writeEnded = true;
                if (_task != null && !_task.IsCompleted)
                {
                    methodLog.Write(LogLevel.Trace, "Waiting 500 ms for task...");
                    _task.Wait(500);
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