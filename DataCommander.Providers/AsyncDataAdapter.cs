using DataCommander.Providers.ResultWriter;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Contracts;
using Foundation.Log;
using Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapter : IAsyncDataAdapter
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private IProvider _provider;
        private IEnumerable<AsyncDataAdapterCommand> _commands;
        private AsyncDataAdapterCommand _command;
        private int _maxRecords;
        private int _rowBlockSize;
        private IResultWriter _resultWriter;
        private Action<IAsyncDataAdapter, Exception> _endFill;
        private Action<IAsyncDataAdapter> _writeEnd;
        private long _rowCount;
        private WorkerThread _thread;
        private int _tableCount;
        private bool _isCommandCanceled;

        #endregion

        public AsyncDataAdapter(
            IProvider provider,
            IEnumerable<AsyncDataAdapterCommand> commands,
            int maxRecords,
            int rowBlockSize,
            IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill,
            Action<IAsyncDataAdapter> writeEnd)
        {
            _provider = provider;
            _commands = commands;
            _maxRecords = maxRecords;
            _rowBlockSize = rowBlockSize;
            _resultWriter = resultWriter;
            _endFill = endFill;
            _writeEnd = writeEnd;
        }

        #region IAsyncDataAdapter Members

        IResultWriter IAsyncDataAdapter.ResultWriter => _resultWriter;

        long IAsyncDataAdapter.RowCount => _rowCount;

        int IAsyncDataAdapter.TableCount => _tableCount;

        void IAsyncDataAdapter.Start()
        {
            if (_commands != null)
            {
                _thread = new WorkerThread(Fill)
                {
                    Name = "AsyncDataAdapter.Fill"
                };

                _thread.Start();
            }
            else
                _writeEnd(this);
        }

        void IAsyncDataAdapter.Cancel()
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                _isCommandCanceled = true;
                if (_thread != null)
                {
                    _thread.Stop();
                    if (_provider.IsCommandCancelable)
                        ThreadPool.QueueUserWorkItem(CancelWaitCallback);
                    else
                    {
                        var joined = _thread.Join(5000);

                        if (!joined)
                        {
                            _thread.Abort();
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void ReadTable(IDataReader dataReader, DataTable schemaTable, int tableIndex)
        {
            FoundationContract.Requires<ArgumentNullException>(dataReader != null);
            FoundationContract.Requires<ArgumentNullException>(schemaTable != null);
            FoundationContract.Requires<ArgumentOutOfRangeException>(tableIndex >= 0);

            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                Exception exception = null;
                var dataReaderHelper = _provider.CreateDataReaderHelper(dataReader);
                var schemaRows = schemaTable.Rows;
                var count = schemaRows.Count;

                _resultWriter.WriteTableBegin(schemaTable);

                var fieldCount = dataReader.FieldCount;

                if (fieldCount < 0)
                    fieldCount = 0;

                var rows = new object[_rowBlockSize][];
                int i;

                for (i = 0; i < _rowBlockSize; i++)
                    rows[i] = new object[fieldCount];

                _rowCount = 0;
                i = 0;
                var first = true;
                var exitFromWhile = false;
                var stopwatch = Stopwatch.StartNew();

                while (!_isCommandCanceled && !_thread.IsStopRequested && !exitFromWhile)
                {
                    bool read;

                    if (first)
                    {
                        first = false;
                        _resultWriter.FirstRowReadBegin();
                        read = dataReader.Read();

                        var dataTypeNames = new string[count];

                        if (read)
                            for (var j = 0; j < count; j++)
                                dataTypeNames[j] = dataReader.GetDataTypeName(j);

                        _resultWriter.FirstRowReadEnd(dataTypeNames);
                    }
                    else
                    {
                        try
                        {
                            read = dataReader.Read();
                        }
                        catch (Exception e)
                        {
                            read = false;
                            exception = e;
                        }
                    }

                    if (read)
                    {
                        _rowCount++;
                        dataReaderHelper.GetValues(rows[i]);
                        i++;

                        if (i == _rowBlockSize || stopwatch.ElapsedMilliseconds >= 5000)
                        {
                            _resultWriter.WriteRows(rows, i);
                            i = 0;
                            stopwatch.Restart();
                        }

                        if (_rowCount == _maxRecords)
                        {
                            CancelWaitCallback(null);
                            break;
                        }
                    }
                    else
                    {
                        exitFromWhile = true;
                    }
                }

                if (i != _rowBlockSize)
                {
                    Log.Trace("resultWriter.WriteRows(rows,i);");
                    _resultWriter.WriteRows(rows, i);
                }

                Log.Write(LogLevel.Trace, "resultWriter.WriteTableEnd(rowCount);");
                _resultWriter.WriteTableEnd();

                if (_rowCount > 0)
                    _tableCount++;

                if (exception != null)
                    throw exception;
            }
        }

        private void Fill(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            FoundationContract.Requires<ArgumentNullException>(asyncDataAdapterCommand != null);

            var command = asyncDataAdapterCommand.Command;

            Exception exception = null;

            try
            {
                _resultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
                IDataReader dataReader = null;
                try
                {
                    dataReader = command.ExecuteReader();
                    var fieldCount = dataReader.FieldCount;
                    _resultWriter.AfterExecuteReader(fieldCount);
                    var tableIndex = 0;

                    while (!_thread.IsStopRequested)
                    {
                        if (fieldCount > 0)
                        {
                            var schemaTable = dataReader.GetSchemaTable();
                            if (schemaTable != null)
                                Log.Trace("schemaTable:\r\n{0}", schemaTable.ToStringTableString());

                            ReadTable(dataReader, schemaTable, tableIndex);
                        }

                        if (_rowCount >= _maxRecords || !dataReader.NextResult())
                            break;

                        tableIndex++;
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        var recordsAffected = dataReader.RecordsAffected;
                        _resultWriter.AfterCloseReader(recordsAffected);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
            catch (Exception e)
            {
                if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                {
                    try
                    {
                        Thread.ResetAbort();
                    }
                    catch
                    {
                    }
                }

                exception = e;
            }
            finally
            {
                if (command != null && command.Parameters != null)
                    _resultWriter.WriteParameters(command.Parameters);

                var ticks = Stopwatch.GetTimestamp();
                _endFill(this, exception);
                ticks = Stopwatch.GetTimestamp() - ticks;
                Log.Write(LogLevel.Trace, "this.endFill( this, exception ); completed in {0} seconds.", StopwatchTimeSpan.ToString(ticks, 3));
            }
        }

        private void Fill()
        {
            _resultWriter.Begin(_provider);

            try
            {
                foreach (var command in _commands)
                {
                    _command = command;
                    Fill(command);
                    command.Command.Dispose();
                }
            }
            finally
            {
                _resultWriter.End();
                _writeEnd(this);
            }
        }

        private void CancelWaitCallback(object state)
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
                _command.Command.Cancel();
        }

        #endregion
    }
}