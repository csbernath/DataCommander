namespace DataCommander.Foundation.Orm
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class OrmContext
    {
        private readonly Func<ITransactionScope> _createTransactionScope;
        private readonly Func<DbConnection> _createConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createTransactionScope"></param>
        /// <param name="createConnection"></param>
        public OrmContext(Func<ITransactionScope> createTransactionScope, Func<DbConnection> createConnection)
        {
            _createTransactionScope = createTransactionScope;
            _createConnection = createConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteActionAsync(Func<DbConnection, Task> action, CancellationToken cancellationToken)
        {
            using (var transactionScopeContext = new TransactionScopeContext(_createTransactionScope))
            {
                using (var connectionContext = new OrmConnectionContext(_createConnection))
                {
                    await connectionContext.OpenAsync(cancellationToken).ConfigureAwait(false);
                    await action(connectionContext.Connection);
                }
                transactionScopeContext.Complete();
            }
        }
    }
}