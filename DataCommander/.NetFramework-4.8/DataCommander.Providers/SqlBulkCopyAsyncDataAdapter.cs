using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataCommander.Providers.ResultWriter;
using DataCommander.Providers2.Connection;

namespace DataCommander.Providers
{
    internal sealed class SqlBulkCopyAsyncDataAdapter : IAsyncDataAdapter
    {
        #region Private Fields

        private readonly Action<InfoMessage> _addInfoMessage;
        private readonly SqlBulkCopy _sqlBulkCopy;
        private long _rowCount;
        private bool _cancelRequested;
        private IDbCommand _command;
        private IProvider _provider;
        private IEnumerable<AsyncDataAdapterCommand> _commands;
        private int _maxRecords;
        private int _rowBlockSize;
        private IResultWriter _resultWriter;
        private Action<IAsyncDataAdapter, Exception> _endFill;
        private Action<IAsyncDataAdapter> _writeEnd;

        #endregion

        public SqlBulkCopyAsyncDataAdapter(SqlConnection destinationConnection, SqlTransaction destionationTransaction, string destinationTableName,
            Action<InfoMessage> addInfoMessage)
        {
            _sqlBulkCopy = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.Default, destionationTransaction);
            _sqlBulkCopy.BulkCopyTimeout = int.MaxValue;
            _sqlBulkCopy.DestinationTableName = destinationTableName;
            _sqlBulkCopy.NotifyAfter = 100000;
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

        #region IAsyncDataAdapter Members

        IResultWriter IAsyncDataAdapter.ResultWriter => throw new NotImplementedException();
        long IAsyncDataAdapter.RowCount => _rowCount;
        int IAsyncDataAdapter.TableCount => 1;

        void IAsyncDataAdapter.Start()
        {
            Task.Factory.StartNew(Fill);
        }

        void IAsyncDataAdapter.Cancel()
        {
            _cancelRequested = true;
            Task.Factory.StartNew(CancelCommand);
        }

        #endregion

        #region Private Methods

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
                    using (var dataReader = _command.ExecuteReader())
                    {
                        _sqlBulkCopy.WriteToServer(dataReader);
                    }
                }
            }
            catch (Exception e)
            {
                exception = e;
            }

            _writeEnd(this);
            _endFill(this, exception);
        }

        private void CancelCommand()
        {
            _command.Cancel();
        }

        #endregion
    }
}