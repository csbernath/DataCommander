namespace DataCommander.Foundation.Data
{
    using System.Data;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(connection != null);
#endif
            this.connection = connection;
            this.transaction = transaction;
        }

        IDbConnection IDbTransactionScope.Connection => this.connection;

        IDbTransaction IDbTransactionScope.Transaction => this.transaction;
    }
}