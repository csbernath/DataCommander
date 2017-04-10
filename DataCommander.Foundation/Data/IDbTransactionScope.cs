namespace DataCommander.Foundation.Data
{
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    //[ContractClass(typeof (IDbTransactionScopeContract))]
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