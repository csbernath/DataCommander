namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DbConnectionContext : IDbConnectionContext
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;
        private readonly int? commandTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        public DbConnectionContext( IDbConnection connection, IDbTransaction transaction, int? commandTimeout )
        {
            Contract.Requires( connection != null );

            this.connection = connection;
            this.transaction = transaction;
            this.commandTimeout = commandTimeout;
        }

        #region IDbConnectionContext Members

        IDbConnection IDbConnectionContext.Connection
        {
            get
            {
                return this.connection;
            }
        }

        IDbTransaction IDbConnectionContext.Transaction
        {
            get
            {
                return this.transaction;
            }
        }

        int? IDbConnectionContext.CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }
        }

        #endregion
    }
}