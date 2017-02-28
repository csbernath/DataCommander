namespace Binarit.Foundation.Data.SqlClient
{
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public interface IDbCommandBuilderHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        void DeriveParameters(IDbCommand command);
    }
}