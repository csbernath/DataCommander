﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;
using Foundation.Log;

namespace DataCommander.Application.ResultWriter;

internal sealed class CopyResultWriter(
    Action<InfoMessage> addInfoMessage,
    IProvider destinationProvider,
    ConnectionBase destinationConnection,
    string? tableName,
    Action<DbTransaction> setTransaction,
    CancellationToken cancellationToken)
    : IResultWriter
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage, false);
    private DbTransaction? _transaction;
    private IDbCommand? _insertCommand;
    private Converter<object, object>?[]? _converters;
    private IDbDataParameter[]? _parameters;
    private ConcurrentQueue<QueueItem>? _queue;
    private Task? _task;
    private EventWaitHandle? _enqueueEvent;
    private bool _writeEnded;
    private readonly bool _canConvertCommandToString = destinationProvider.CanConvertCommandToString;
    private long _readRowCount;
    private long _insertedRowCount;
    private long _waitMilliseconds;

    void IResultWriter.Begin(IProvider provider) => _logResultWriter.Begin(provider);

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command) => _logResultWriter.BeforeExecuteReader(command);
    void IResultWriter.AfterExecuteReader() => _logResultWriter.AfterExecuteReader();
    void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        _logResultWriter.WriteTableBegin(schemaTable);
        destinationProvider.CreateInsertCommand(schemaTable, null, destinationConnection.Connection!, tableName, out _insertCommand,
            out _converters);
        //  TODO this.messageWriter.WriteLine( this.insertCommand.CommandText );
        _parameters = _insertCommand.Parameters.Cast<IDbDataParameter>().ToArray();
        if (_transaction == null)
        {
            _transaction = destinationConnection.Connection!.BeginTransaction();
            setTransaction(_transaction);
        }

        _insertCommand.Transaction = _transaction;
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
                    var converter = _converters![columnIndex];
                    object destinationValue;

                    if (converter != null)
                    {
                        destinationValue = converter(sourceValue);
                    }
                    else
                    {
                        destinationValue = sourceValue;
                    }

                    _parameters![columnIndex].Value = destinationValue;
                }

                if (_canConvertCommandToString)
                {
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                    }

                    var commandText = destinationProvider.CommandToString(_insertCommand!);
                    sb.Append(commandText);
                }
                else
                {
                    _insertCommand!.ExecuteNonQuery();
                }

                _insertedRowCount++;
            }
        }

        if (sb.Length > 0)
        {
            var commandText = sb.ToString();
            try
            {
                var executor = destinationConnection.Connection.CreateCommandExecutor();
                executor.ExecuteNonQuery(new CreateCommandRequest(commandText, null, CommandType.Text, 3600, _insertCommand!.Transaction));
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, "CommandText:\r\n{0}\r\nException:{1}", commandText, e.ToLogString());
                throw;
            }
        }

        var message =
            $"{_readRowCount},{_insertedRowCount},{_readRowCount - _insertedRowCount},{_waitMilliseconds} (rows read,inserted,queued,wait).";

        addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message));
    }

    private void Dequeue()
    {
        try
        {
            using var methodLog = LogFactory.Instance.GetCurrentMethodLog();
            while (true)
            {
                methodLog.Write(LogLevel.Trace, "this.queue.Count: {0}", _queue!.Count);
                if (!_queue.IsEmpty)
                {
                    var items = new List<QueueItem>(_queue.Count);
                    while (true)
                    {
                        var succeeded = _queue.TryDequeue(out var item);
                        if (succeeded)
                            items.Add(item!);
                        else
                            break;
                    }

                    InsertItems(items);
                }

                if (_queue.IsEmpty)
                {
                    methodLog.Write(LogLevel.Trace, "this.writeEnded: {0}", _writeEnded);
                    if (_writeEnded)
                    {
                        break;
                    }
                    else
                    {
                        methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 );...");
                        _enqueueEvent!.WaitOne(1000);
                        methodLog.Write(LogLevel.Trace, "this.enqueueEvent.WaitOne( 1000 ); finished.");
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

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);
        _readRowCount += rowCount;

        var message = $"{_readRowCount},{_insertedRowCount},{_readRowCount - _insertedRowCount} (rows read,inserted,queued).";

        addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message));
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

        _queue!.Enqueue(queueItem);
        _enqueueEvent!.Set();

        while (!cancellationToken.IsCancellationRequested && _queue.Count > 5)
        {
            _waitMilliseconds += 500;
            Log.Write(LogLevel.Trace, "this.waitMilliseconds: {0}", _waitMilliseconds);

            cancellationToken.WaitHandle.WaitOne(500);
        }
    }

    void IResultWriter.WriteTableEnd() => _logResultWriter.WriteTableEnd();

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
    }

    void IResultWriter.End()
    {
        _logResultWriter.End();
        using var methodLog = LogFactory.Instance.GetCurrentMethodLog();
        _writeEnded = true;
        if (_task != null && !_task.IsCompleted)
        {
            methodLog.Write(LogLevel.Trace, "Waiting 500 ms for task...");
            _task.Wait(500);
        }
    }

    private sealed class QueueItem
    {
        public object[][] Rows;
    }
}