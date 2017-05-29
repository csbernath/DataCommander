using System.Data;

namespace Foundation.Data
{
    //[ContractClassFor(typeof (IDbTransactionScope))]
    internal abstract class DbTransactionScopeContract : IDbTransactionScope
    {
        IDbConnection IDbTransactionScope.Connection
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Ensures(Contract.Result<IDbConnection>() != null);
#endif
                return null;
            }
        }

        IDbTransaction IDbTransactionScope.Transaction => null;
    }
}