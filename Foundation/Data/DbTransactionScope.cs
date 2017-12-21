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
            FoundationContract.Requires<ArgumentNullException>(connection != null);
#endif
            _connection = connection;
            _transaction = transaction;
        }

        IDbConnection IDbTransactionScope.Connection => _connection;

        IDbTransaction IDbTransactionScope.Transaction => _transaction;
    }
}