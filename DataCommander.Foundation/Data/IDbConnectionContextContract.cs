namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    [ContractClassFor( typeof(IDbConnectionContext) )]
    internal abstract class IDbConnectionContextContract : IDbConnectionContext
    {
        #region IDbConnectionContext Members

        IDbConnection IDbConnectionContext.Connection
        {
            get
            {
                Contract.Ensures( Contract.Result<IDbConnection>() != null );
                return null;
            }
        }

        IDbTransaction IDbConnectionContext.Transaction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Int32? IDbConnectionContext.CommandTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}