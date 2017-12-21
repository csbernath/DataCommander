using System.Data;
using System.Data.SqlClient;

namespace Foundation.Data.SqlClient
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SqlCommandBuilderHelper : IDbCommandBuilderHelper
    {
        private SqlCommandBuilderHelper()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static SqlCommandBuilderHelper Instance { get; } = new SqlCommandBuilderHelper();

        #region IDbCommandBuilder Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void DeriveParameters(IDbCommand command)
        {
#if CONTRACTS_FULL
            FoundationContract.Assert(command != null);
#endif

            var sqlCommand = (SqlCommand)command;
            SqlCommandBuilder.DeriveParameters(sqlCommand);
        }

#endregion
    }
}