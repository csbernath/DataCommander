namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DbTransactionScope : IDbTransactionScope
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public DbTransactionScope(IDbConnection connection, IDbTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            this.connection = connection;
            this.transaction = transaction;
        }

        IDbConnection IDbTransactionScope.Connection
        {
            get
            {
                return this.connection;
            }
        }

        IDbTransaction IDbTransactionScope.Transaction
        {
            get
            {
                return this.transaction;
            }
        }
    }
}