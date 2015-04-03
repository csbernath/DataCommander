namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    [ContractClass( typeof(IDbConnectionContextContract) )]

    public interface IDbConnectionContext
    {
        /// <summary>
        /// 
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// 
        /// </summary>
        int? CommandTimeout { get; }
    }
}