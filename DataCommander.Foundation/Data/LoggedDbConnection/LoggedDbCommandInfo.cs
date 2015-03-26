namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbCommandInfo
    {
        private readonly int commandId;
        private readonly ConnectionState connectionState;
        private LoggedDbCommandExecutionType executionType;
        private CommandType commandType;
        private string database;
        private int commandTimeout;
        private string commandText;
        private string parameters;

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
        public int CommandId
        {
            get
            {
                return this.commandId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState ConnectionState
        {
            get
            {
                return this.connectionState;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Database
        {
            get
            {
                return this.database;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandExecutionType ExecutionType
        {
            get
            {
                return this.executionType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return this.commandType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}
