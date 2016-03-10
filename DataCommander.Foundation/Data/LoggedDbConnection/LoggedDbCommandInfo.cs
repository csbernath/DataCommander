namespace DataCommander.Foundation.Data
{
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbCommandInfo
    {
        private readonly int commandId;
        private readonly ConnectionState connectionState;
        private readonly LoggedDbCommandExecutionType executionType;
        private readonly CommandType commandType;
        private readonly string database;
        private readonly int commandTimeout;
        private readonly string commandText;
        private readonly string parameters;

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
            this.commandId = commandId;
            this.connectionState = connectionState;
            this.database = database;
            this.executionType = executionType;
            this.commandType = commandType;
            this.commandText = commandText;
            this.commandTimeout = commandTimeout;
            this.parameters = parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandId => this.commandId;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState ConnectionState => this.connectionState;

        /// <summary>
        /// 
        /// </summary>
        public string Database => this.database;

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandExecutionType ExecutionType => this.executionType;

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType => this.commandType;

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout => this.commandTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string CommandText => this.commandText;

        /// <summary>
        /// 
        /// </summary>
        public string Parameters => this.parameters;
    }
}
