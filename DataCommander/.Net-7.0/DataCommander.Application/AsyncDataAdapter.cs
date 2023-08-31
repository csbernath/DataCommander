using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.QueryConfiguration;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Log;

namespace DataCommander.Application;

internal sealed class AsyncDataAdapter : IAsyncDataAdapter
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    private readonly IProvider _provider;
    private readonly IReadOnlyCollection<AsyncDataAdapterCommand> _commands;
    private readonly int _maxRecords;
    private readonly int _rowBlockSize;
    private readonly IResultWriter _resultWriter;
    private readonly Action<IAsyncDataAdapter, Exception> _endFill;
    private readonly Action<IAsyncDataAdapter> _writeEnd;

    private AsyncDataAdapterCommand _command;
    private long _rowCount;
    private Task? _task;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private int _tableCount;
    private bool _isCommandCancelled;

    #endregion

    public AsyncDataAdapter(IProvider provider, IReadOnlyCollection<AsyncDataAdapterCommand> commands, int maxRecords, int rowBlockSize, IResultWriter resultWriter,
        Action<IAsyncDataAdapter, Exception> endFill, Action<IAsyncDataAdapter> writeEnd)
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
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _task = new Task(async () => await Fill(), _cancellationToken, TaskCreationOptions.LongRunning);
            _task.Start();
        }
        else
            _writeEnd(this);
    }

    void IAsyncDataAdapter.Cancel()
    {
        using (LogFactory.Instance.GetCurrentMethodLog())
        {
            _isCommandCancelled = true;
            _cancellationTokenSource.Cancel();
            if (_provider.IsCommandCancelable)
            {
                var task = new Task(() => { _command.Command.Cancel(); });
                task.Start();
            }
        }
    }

    #endregion

    #region Private Methods

    private void ReadTable(DbDataReader dataReader, DataTable schemaTable, int tableIndex)
    {
        ArgumentNullException.ThrowIfNull(dataReader);
        ArgumentNullException.ThrowIfNull(schemaTable);
        Assert.IsInRange(tableIndex >= 0);

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

            while (!_isCommandCancelled && !_cancellationTokenSource.IsCancellationRequested && !exitFromWhile)
            {
                bool read;

                if (first)
                {
                    first = false;
                    _resultWriter.FirstRowReadBegin();
                    read = dataReader.Read();

                    var dataTypeNames = new string[count];

                    if (read)
                        for (var j = 0; j < count; ++j)
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
                    ++_rowCount;
                    dataReaderHelper.GetValues(rows[i]);
                    ++i;

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
                    exitFromWhile = true;
            }

            if (i != _rowBlockSize)
            {
                Log.Trace("resultWriter.WriteRows(rows,i);");
                _resultWriter.WriteRows(rows, i);
            }

            Log.Write(LogLevel.Trace, "resultWriter.WriteTableEnd(rowCount);");
            _resultWriter.WriteTableEnd();

            if (_rowCount > 0)
                ++_tableCount;

            if (exception != null)
                throw exception;
        }
    }

    private async Task Fill(AsyncDataAdapterCommand asyncDataAdapterCommand)
    {
        ArgumentNullException.ThrowIfNull(asyncDataAdapterCommand);

        Exception exception = null;
        var command = asyncDataAdapterCommand.Command;

        try
        {
            await ExecuteReader(asyncDataAdapterCommand, command);
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            if (command != null && command.Parameters != null)
                _resultWriter.WriteParameters(command.Parameters);

            _endFill(this, exception);
        }
    }

    private async Task ExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand, DbCommand command)
    {
        _resultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
        DbDataReader dataReader = null;
        try
        {
            dataReader = await command.ExecuteReaderAsync(_cancellationToken);
            var fieldCount = dataReader.FieldCount;
            _resultWriter.AfterExecuteReader();
            var tableIndex = 0;

            while (!_cancellationToken.IsCancellationRequested)
            {
                if (fieldCount > 0)
                {
                    var schemaTable = dataReader.GetSchemaTable();
                    if (schemaTable != null)
                    {
                        Log.Trace("schemaTable:\r\n{0}", schemaTable.ToStringTableString());
                        if (asyncDataAdapterCommand.Query != null)
                        {
                            Parser.ParseResult(asyncDataAdapterCommand.Query.Results[tableIndex], out var name, out var fieldName);
                            schemaTable.TableName = name;
                        }
                    }

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

    private async Task Fill()
    {
        _resultWriter.Begin(_provider);

        try
        {
            foreach (var command in _commands)
            {
                _command = command;
                await Fill(command);
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