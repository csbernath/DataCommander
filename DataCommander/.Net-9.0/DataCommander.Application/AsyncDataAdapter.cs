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

internal sealed class AsyncDataAdapter(
    IProvider provider,
    int maxRecords,
    int rowBlockSize,
    IResultWriter resultWriter,
    Action<IAsyncDataAdapter, Exception> endFill,
    Action<IAsyncDataAdapter> writeEnd)
    : IAsyncDataAdapter
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    private AsyncDataAdapterCommand _command;
    private long _rowCount;
    private CancellationTokenSource _cancellationTokenSource;
    private int _tableCount;

    IResultWriter IAsyncDataAdapter.ResultWriter => resultWriter;
    long IAsyncDataAdapter.RowCount => _rowCount;
    int IAsyncDataAdapter.TableCount => _tableCount;

    public void Start(IEnumerable<AsyncDataAdapterCommand> commands)
    {
        if (commands != null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            var task = new Task(async () => await Fill(commands, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning);
            task.Start();
        }
        else
            writeEnd(this);
    }

    void IAsyncDataAdapter.Cancel()
    {
        using (LogFactory.Instance.GetCurrentMethodLog())
        {
            _cancellationTokenSource.Cancel();
            if (provider.IsCommandCancelable)
            {
                var task = new Task(() => { _command.Command.Cancel(); });
                task.Start();
            }
        }
    }

    private async Task ReadTable(DbDataReader dataReader, DataTable schemaTable, int tableIndex, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dataReader);
        ArgumentNullException.ThrowIfNull(schemaTable);
        Assert.IsInRange(tableIndex >= 0);

        using (LogFactory.Instance.GetCurrentMethodLog())
        {
            Exception exception = null;
            var dataReaderHelper = provider.CreateDataReaderHelper(dataReader);
            var schemaRows = schemaTable.Rows;
            var count = schemaRows.Count;

            resultWriter.WriteTableBegin(schemaTable);

            var fieldCount = dataReader.FieldCount;

            if (fieldCount < 0)
                fieldCount = 0;

            object[][] rows = new object[rowBlockSize][];
            int i;

            for (i = 0; i < rowBlockSize; i++)
                rows[i] = new object[fieldCount];

            _rowCount = 0;
            i = 0;
            var first = true;
            var exitFromWhile = false;
            var stopwatch = Stopwatch.StartNew();

            while (!cancellationToken.IsCancellationRequested && !exitFromWhile)
            {
                bool read;

                if (first)
                {
                    first = false;
                    resultWriter.FirstRowReadBegin();
                    read = await dataReader.ReadAsync(cancellationToken);

                    string[] dataTypeNames = new string[count];

                    if (read)
                        for (var j = 0; j < count; ++j)
                            dataTypeNames[j] = dataReader.GetDataTypeName(j);

                    resultWriter.FirstRowReadEnd(dataTypeNames);
                }
                else
                {
                    try
                    {
                        read = await dataReader.ReadAsync(cancellationToken);
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

                    if (i == rowBlockSize || stopwatch.ElapsedMilliseconds >= 5000)
                    {
                        resultWriter.WriteRows(rows, i);
                        i = 0;
                        stopwatch.Restart();
                    }

                    if (_rowCount == maxRecords)
                    {
                        CancelWaitCallback(null);
                        break;
                    }
                }
                else
                    exitFromWhile = true;
            }

            if (i != rowBlockSize)
            {
                Log.Trace("resultWriter.WriteRows(rows,i);");
                resultWriter.WriteRows(rows, i);
            }

            Log.Write(LogLevel.Trace, "resultWriter.WriteTableEnd(rowCount);");
            resultWriter.WriteTableEnd();

            if (_rowCount > 0)
                ++_tableCount;

            if (exception != null)
                throw exception;
        }
    }

    private async Task Fill(AsyncDataAdapterCommand asyncDataAdapterCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(asyncDataAdapterCommand);
        Exception exception = null;
        var command = asyncDataAdapterCommand.Command;

        try
        {
            await ExecuteReader(asyncDataAdapterCommand, command, cancellationToken);
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            if (command != null && command.Parameters != null)
                resultWriter.WriteParameters(command.Parameters);

            endFill(this, exception);
        }
    }

    private async Task ExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand, DbCommand command, CancellationToken cancellationToken)
    {
        resultWriter.BeforeExecuteReader(asyncDataAdapterCommand);
        DbDataReader? dataReader = null;
        try
        {
            _command = asyncDataAdapterCommand;
            dataReader = await command.ExecuteReaderAsync(cancellationToken);
            var fieldCount = dataReader.FieldCount;
            resultWriter.AfterExecuteReader();
            var tableIndex = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (fieldCount > 0)
                {
                    var schemaTable = await dataReader.GetSchemaTableAsync(cancellationToken);
                    if (schemaTable != null)
                    {
                        Log.Trace($"schemaTable:\r\n{schemaTable.ToStringTableString()}");
                        if (asyncDataAdapterCommand.Query != null)
                        {
                            Parser.ParseResult(asyncDataAdapterCommand.Query.Results[tableIndex], out var name, out var fieldName);
                            schemaTable.TableName = name;
                        }
                    }

                    await ReadTable(dataReader, schemaTable, tableIndex, cancellationToken);
                }

                if (_rowCount >= maxRecords || !await dataReader.NextResultAsync(cancellationToken))
                    break;

                tableIndex++;
            }
        }
        finally
        {
            if (dataReader != null)
            {
                await dataReader.CloseAsync();
                var recordsAffected = dataReader.RecordsAffected;
                resultWriter.AfterCloseReader(recordsAffected);
            }
        }
    }

    private async Task Fill(IEnumerable<AsyncDataAdapterCommand> commands, CancellationToken cancellationToken)
    {
        resultWriter.Begin(provider);

        try
        {
            foreach (var command in commands)
            {
                await Fill(command, cancellationToken);
                command.Command.Dispose();
            }
        }
        finally
        {
            resultWriter.End();
            writeEnd(this);
        }
    }

    private void CancelWaitCallback(object state)
    {
        using (LogFactory.Instance.GetCurrentMethodLog())
            _command.Command.Cancel();
    }
}