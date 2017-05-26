using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using DataCommander.Providers.ResultWriter;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Foundation.Threading;
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

        #region IAsyncDataAdapter Members

        IResultWriter IAsyncDataAdapter.ResultWriter => this._resultWriter;

        long IAsyncDataAdapter.RowCount => this._rowCount;

        int IAsyncDataAdapter.TableCount => this._tableCount;

        void IAsyncDataAdapter.BeginFill(
            IProvider provider,
            IEnumerable<AsyncDataAdapterCommand> commands,
            int maxRecords,
            int rowBlockSize,
            IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill,
            Action<IAsyncDataAdapter> writeEnd)
        {
            this._provider = provider;
            this._commands = commands;
            this._maxRecords = maxRecords;
            this._rowBlockSize = rowBlockSize;
            this._resultWriter = resultWriter;
            this._endFill = endFill;
            this._writeEnd = writeEnd;

            if (commands != null)
            {
                this._thread = new WorkerThread(this.Fill)
                {
                    Name = "AsyncDataAdapter.Fill"
                };

                this._thread.Start();
            }
            else
            {
                writeEnd(this);
            }
        }

        void IAsyncDataAdapter.Cancel()
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                this._isCommandCanceled = true;
                if (this._thread != null)
                {
                    this._thread.Stop();
                    if (this._provider.IsCommandCancelable)
                    {
                        ThreadPool.QueueUserWorkItem(this.CancelWaitCallback);
                    }
                    else
                    {
                        var joined = this._thread.Join(5000);

                        if (!joined)
                        {
                            this._thread.Abort();
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void ReadTable(IDataReader dataReader, DataTable schemaTable, int tableIndex)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(schemaTable != null);
            Contract.Requires<ArgumentOutOfRangeException>(tableIndex >= 0);
#endif

            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                Exception exception = null;
                var dataReaderHelper = this._provider.CreateDataReaderHelper(dataReader);
                var schemaRows = schemaTable.Rows;
                var count = schemaRows.Count;

                this._resultWriter.WriteTableBegin(schemaTable);

                var fieldCount = dataReader.FieldCount;

                if (fieldCount < 0)
                {
                    fieldCount = 0;
                }

                var rows = new object[this._rowBlockSize][];
                int i;

                for (i = 0; i < this._rowBlockSize; i++)
                {
                    rows[i] = new object[fieldCount];
                }

                this._rowCount = 0;
                i = 0;
                var first = true;
                var exitFromWhile = false;
                var stopwatch = Stopwatch.StartNew();

                while (!this._isCommandCanceled && !this._thread.IsStopRequested && !exitFromWhile)
                {
                    bool read;

                    if (first)
                    {
                        first = false;
                        this._resultWriter.FirstRowReadBegin();
                        read = dataReader.Read();

                        var dataTypeNames = new string[count];

                        if (read)
                        {
                            for (var j = 0; j < count; j++)
                            {
                                dataTypeNames[j] = dataReader.GetDataTypeName(j);
                            }
                        }

                        this._resultWriter.FirstRowReadEnd(dataTypeNames);
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
                        this._rowCount++;
                        dataReaderHelper.GetValues(rows[i]);
                        i++;

                        if (i == this._rowBlockSize || stopwatch.ElapsedMilliseconds >= 5000)
                        {
                            this._resultWriter.WriteRows(rows, i);
                            i = 0;
                            stopwatch.Restart();
                        }

                        if (this._rowCount == this._maxRecords)
                        {
                            this.CancelWaitCallback(null);
                            break;
                        }
                    }
                    else
                    {
                        exitFromWhile = true;
                    }
                }

                if (i != this._rowBlockSize)
                {
                    Log.Write(LogLevel.Trace, "resultWriter.WriteRows(rows,i);");
                    this._resultWriter.WriteRows(rows, i);
                }

                Log.Write(LogLevel.Trace, "resultWriter.WriteTableEnd(rowCount);");
                this._resultWriter.WriteTableEnd();

                if (this._rowCount > 0)
                {
                    this._tableCount++;
                }

                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        private void Fill(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(asyncDataAdapterCommand != null);
#endif
            var command = asyncDataAdapterCommand.Command;

            Exception exception = null;

            try
            {
                this._resultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
                IDataReader dataReader = null;
                try
                {
                    dataReader = command.ExecuteReader();
                    var fieldCount = dataReader.FieldCount;
                    this._resultWriter.AfterExecuteReader(fieldCount);
                    var tableIndex = 0;

                    while (!this._thread.IsStopRequested)
                    {
                        if (fieldCount > 0)
                        {
                            var schemaTable = dataReader.GetSchemaTable();
                            if (schemaTable != null)
                            {
                                Log.Trace("schemaTable:\r\n{0}", schemaTable.ToStringTableString());
                            }

                            this.ReadTable(dataReader, schemaTable, tableIndex);
                        }

                        if (this._rowCount >= this._maxRecords || !dataReader.NextResult())
                        {
                            break;
                        }

                        tableIndex++;
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                        var recordsAffected = dataReader.RecordsAffected;
                        this._resultWriter.AfterCloseReader(recordsAffected);
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
                {
                    this._resultWriter.WriteParameters(command.Parameters);
                }
                var ticks = Stopwatch.GetTimestamp();
                this._endFill(this, exception);
                ticks = Stopwatch.GetTimestamp() - ticks;
                Log.Write(LogLevel.Trace, "this.endFill( this, exception ); completed in {0} seconds.", StopwatchTimeSpan.ToString(ticks, 3));
            }
        }

        private void Fill()
        {
            this._resultWriter.Begin(this._provider);

            try
            {
                foreach (var command in this._commands)
                {
                    this._command = command;
                    this.Fill(command);
                    command.Command.Dispose();
                }
            }
            finally
            {
                this._resultWriter.End();
                this._writeEnd(this);
            }
        }

        private void CancelWaitCallback(object state)
        {
            using (LogFactory.Instance.GetCurrentMethodLog())
            {
                this._command.Command.Cancel();
            }
        }

        #endregion
    }
}