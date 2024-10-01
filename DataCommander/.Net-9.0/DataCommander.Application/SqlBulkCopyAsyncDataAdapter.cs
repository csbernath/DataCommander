using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Application;

internal sealed class SqlBulkCopyAsyncDataAdapter : IAsyncDataAdapter
{
    private readonly Action<InfoMessage> _addInfoMessage;
    private readonly SqlBulkCopy _sqlBulkCopy;
    private long _rowCount;
    private bool _cancelRequested;
    private IDbCommand _command;
    private readonly IProvider _provider;
    private readonly IEnumerable<AsyncDataAdapterCommand> _commands;
    private readonly int _maxRecords;
    private readonly int _rowBlockSize;
    private readonly IResultWriter _resultWriter;
    private readonly Action<IAsyncDataAdapter, Exception> _endFill;
    private readonly Action<IAsyncDataAdapter> _writeEnd;

    public SqlBulkCopyAsyncDataAdapter(SqlConnection destinationConnection, SqlTransaction destionationTransaction, string destinationTableName,
        Action<InfoMessage> addInfoMessage)
    {
        _sqlBulkCopy = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.Default, destionationTransaction)
        {
            BulkCopyTimeout = int.MaxValue,
            DestinationTableName = destinationTableName,
            NotifyAfter = 100000
        };
        _sqlBulkCopy.SqlRowsCopied += sqlBulkCopy_SqlRowsCopied;
        _addInfoMessage = addInfoMessage;
    }

    private void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        _rowCount += e.RowsCopied;
        var message = $"{_rowCount} rows copied.";
        _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message));
        if (_cancelRequested)
        {
            _addInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, "Aborting bulk copy..."));
            e.Abort = true;
        }
    }

    IResultWriter IAsyncDataAdapter.ResultWriter => throw new NotImplementedException();
    long IAsyncDataAdapter.RowCount => _rowCount;
    int IAsyncDataAdapter.TableCount => 1;

    void IAsyncDataAdapter.Start(IEnumerable<AsyncDataAdapterCommand> commands) => Task.Factory.StartNew(Fill);

    void IAsyncDataAdapter.Cancel()
    {
        _cancelRequested = true;
        Task.Factory.StartNew(CancelCommand);
    }

    private void Fill()
    {
        Exception exception = null;
        try
        {
            foreach (var command in _commands)
            {
                if (_cancelRequested)
                {
                    break;
                }

                _command = command.Command;
                using var dataReader = _command.ExecuteReader();
                _sqlBulkCopy.WriteToServer(dataReader);
            }
        }
        catch (Exception e)
        {
            exception = e;
        }

        _writeEnd(this);
        _endFill(this, exception);
    }

    private void CancelCommand() => _command.Cancel();
}