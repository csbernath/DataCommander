namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    [ContractClass(typeof (IDbTransactionScopeContract))]
    public interface IDbTransactionScope
    {
        /// <summary>
        /// 
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        IDbTransaction Transaction { get; }
    }
}