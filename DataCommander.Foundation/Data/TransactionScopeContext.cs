namespace DataCommander.Foundation.Data
{
    using System;

    internal sealed class TransactionScopeContext : IDisposable
    {
        private ITransactionScope _transactionScope;

        public TransactionScopeContext(Func<ITransactionScope> createTransactionScope)
        {
            if (createTransactionScope != null)
                _transactionScope = createTransactionScope();
        }

        public void Complete()
        {
            if (_transactionScope != null)
                _transactionScope.Complete();
        }

        void IDisposable.Dispose()
        {
            if (_transactionScope != null)
                _transactionScope.Dispose();
        }
    }
}