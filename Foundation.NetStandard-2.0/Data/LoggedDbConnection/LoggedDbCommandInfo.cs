using System.Data;

namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbCommandInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="connectionState"></param>
        /// <param name="database"></param>
        /// <param name="executionType"></param>
        /// <param name="commandType"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        public LoggedDbCommandInfo(
            int commandId,
            ConnectionState connectionState,
            string database,
            LoggedDbCommandExecutionType executionType,
            CommandType commandType,
            int commandTimeout,
            string commandText,
            string parameters )
        {
            CommandId = commandId;
            ConnectionState = connectionState;
            Database = database;
            ExecutionType = executionType;
            CommandType = commandType;
            CommandText = commandText;
            CommandTimeout = commandTimeout;
            Parameters = parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandId { get; }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState ConnectionState { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandExecutionType ExecutionType { get; }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType { get; }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout { get; }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Parameters { get; }
    }
}
