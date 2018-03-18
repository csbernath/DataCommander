using System.Data;
using System.Diagnostics.Contracts;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    //[ContractClassFor(typeof (IDbTransactionScope))]
    internal abstract class DbTransactionScopeContract : IDbTransactionScope
    {
        IDbConnection IDbTransactionScope.Connection
        {
            get
            {
                FoundationContract.Ensures(Contract.Result<IDbConnection>() != null);
                return null;
            }
        }

        IDbTransaction IDbTransactionScope.Transaction => null;
    }
}