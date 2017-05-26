using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DataCommander.Providers.Connection;
using DataCommander.Providers.ResultWriter;
using Foundation;

namespace DataCommander.Providers
{
    internal sealed class SqlBulkCopyAsyncDataAdapter : IAsyncDataAdapter
    {
        #region Private Fields

        private readonly Action<InfoMessage> addInfoMessage;
        private readonly SqlBulkCopy sqlBulkCopy;
        private long rowCount;
        private bool cancelRequested;
        private IDbCommand command;
        private IProvider provider;
        private IEnumerable<AsyncDataAdapterCommand> commands;
        private int maxRecords;
        private int rowBlockSize;
        private IResultWriter resultWriter;
        private Action<IAsyncDataAdapter, Exception> endFill;
        private Action<IAsyncDataAdapter> writeEnd;

        #endregion

        public SqlBulkCopyAsyncDataAdapter(
            SqlConnection destinationConnection,
            SqlTransaction destionationTransaction,
            string destinationTableName,
            Action<InfoMessage> addInfoMessage)
        {
            this.sqlBulkCopy = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.Default, destionationTransaction);
            this.sqlBulkCopy.BulkCopyTimeout = int.MaxValue;
            this.sqlBulkCopy.DestinationTableName = destinationTableName;
            this.sqlBulkCopy.NotifyAfter = 100000;
            this.sqlBulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(this.sqlBulkCopy_SqlRowsCopied);
            this.addInfoMessage = addInfoMessage;
        }

        private void sqlBulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            this.rowCount += e.RowsCopied;
            var message = $"{this.rowCount} rows copied.";
            this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            if (this.cancelRequested)
            {
                this.addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, "Aborting bulk copy..."));
                e.Abort = true;
            }
        }

        #region IAsyncDataAdapter Members

        IResultWriter IAsyncDataAdapter.ResultWriter => throw new NotImplementedException();

        long IAsyncDataAdapter.RowCount => this.rowCount;

        int IAsyncDataAdapter.TableCount => 1;

        void IAsyncDataAdapter.BeginFill(IProvider provider, IEnumerable<AsyncDataAdapterCommand> commands, int maxRecords, int rowBlockSize,
            IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill, Action<IAsyncDataAdapter> writeEnd)
        {
            this.provider = provider;
            this.commands = commands;
            this.maxRecords = maxRecords;
            this.rowBlockSize = rowBlockSize;
            this.resultWriter = resultWriter;
            this.endFill = endFill;
            this.writeEnd = writeEnd;

            Task.Factory.StartNew(this.Fill);
        }

        void IAsyncDataAdapter.Cancel()
        {
            this.cancelRequested = true;
            Task.Factory.StartNew(this.CancelCommand);
        }

        #endregion

        #region Private Methods

        private void Fill()
        {
            Exception exception = null;
            try
            {
                foreach (var command in this.commands)
                {
                    if (this.cancelRequested)
                    {
                        break;
                    }

                    this.command = command.Command;
                    using (var dataReader = this.command.ExecuteReader())
                    {
                        this.sqlBulkCopy.WriteToServer(dataReader);
                    }
                }
            }
            catch (Exception e)
            {
                exception = e;
            }

            this.writeEnd(this);
            this.endFill(this, exception);
        }

        private void CancelCommand()
        {
            this.command.Cancel();
        }

        #endregion
    }
}