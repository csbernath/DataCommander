namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;

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
        public void DeriveParameters( IDbCommand command )
        {
            Contract.Assert( command != null );

            var sqlCommand = (SqlCommand)command;
            SqlCommandBuilder.DeriveParameters( sqlCommand );
        }

        #endregion
    }
}