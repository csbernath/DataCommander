namespace DataCommander.Foundation.Data
{
    using DataCommander.Foundation.Data.SqlClient;

    /// <summary>
    /// 
    /// </summary>
    public interface IDbProviderFactoryHelper
    {
        /// <summary>
        /// 
        /// </summary>
        IDbCommandHelper DbCommandHelper { get; }

        /// <summary>
        /// 
        /// </summary>
        IDbCommandBuilderHelper DbCommandBuilderHelper { get; }
    }
}