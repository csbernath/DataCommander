namespace DataCommander.Foundation.Data
{
    using System.Data;

    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof (IDbTransactionScope))]
    internal abstract class IDbTransactionScopeContract : IDbTransactionScope
    {
        IDbConnection IDbTransactionScope.Connection
        {
            get
            {
                Contract.Ensures(Contract.Result<IDbConnection>() != null);
                return null;
            }
        }

        IDbTransaction IDbTransactionScope.Transaction => null;
    }
}