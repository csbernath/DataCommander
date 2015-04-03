namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IDbConnectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout)
        {
            Contract.Requires(connection != null);

            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (commandTimeout != null)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout )
        {
            DataSet dataSet;

            using (var command = connection.CreateCommand( transaction, commandText, commandType, commandTimeout ))
            {
                dataSet = command.ExecuteDataSet();
            }

            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout )
        {
            DataTable dataTable;

            using (var command = connection.CreateCommand( transaction, commandText, commandType, commandTimeout ))
            {
                dataTable = command.ExecuteDataTable();
            }

            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout )
        {
            int affectedRowCount;

            using (var command = connection.CreateCommand( transaction, commandText, commandType, commandTimeout ))
            {
                affectedRowCount = command.ExecuteNonQuery();
            }

            return affectedRowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandBehavior"></param>
        /// <returns></returns>
        public static IDataReaderContext ExecuteReader(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout,
            CommandBehavior commandBehavior )
        {
            var command = connection.CreateCommand( transaction, commandText, commandType, commandTimeout );
            var dataReader = command.ExecuteReader( commandBehavior );
            return new DataReaderContext( command, dataReader );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static object ExecuteScalar(
            this IDbConnection connection,
            IDbTransaction transaction,
            string commandText,
            CommandType commandType,
            int? commandTimeout )
        {
            Contract.Requires( connection != null );

            object scalar;

            using (var command = connection.CreateCommand( transaction, commandText, commandType, commandTimeout ))
            {
                scalar = command.ExecuteScalar();
            }

            return scalar;
        }
    }
}