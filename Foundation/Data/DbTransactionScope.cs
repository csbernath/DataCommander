using System.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(connection);

            _connection = connection;
            _transaction = transaction;
        }

        IDbConnection IDbTransactionScope.Connection => _connection;

        IDbTransaction IDbTransactionScope.Transaction => _transaction;
    }
}