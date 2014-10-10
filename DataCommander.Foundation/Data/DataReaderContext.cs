namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal sealed class DataReaderContext : IDataReaderContext
    {
        private readonly IDbCommand command;
        private readonly IDataReader dataReader;

        public DataReaderContext( IDbCommand command, IDataReader dataReader )
        {
            Contract.Requires( command != null );
            Contract.Requires( dataReader != null );

            this.command = command;
            this.dataReader = dataReader;
        }
      
        #region IDbCommandContext Members

        IDbCommand IDataReaderContext.Command
        {
            get
            {
                return this.command;
            }
        }

        IDataReader IDataReaderContext.DataReader
        {
            get
            {
                return this.dataReader;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.dataReader.Dispose();
            this.command.Dispose();
        }

        #endregion
    }
}