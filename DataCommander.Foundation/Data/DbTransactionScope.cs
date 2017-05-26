using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DbTransactionScope : IDbTransactionScope
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public DbTransactionScope(IDbConnection connection, IDbTransaction transaction)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(connection != null);
#endif
            this._connection = connection;
            this._transaction = transaction;
        }

        IDbConnection IDbTransactionScope.Connection => this._connection;

        IDbTransaction IDbTransactionScope.Transaction => this._transaction;
    }
}