namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LoggedDbCommandInfo
    {
        private readonly Int32 commandId;
        private readonly ConnectionState connectionState;
        private LoggedDbCommandExecutionType executionType;
        private CommandType commandType;
        private String database;
        private Int32 commandTimeout;
        private String commandText;
        private String parameters;

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
            Int32 commandId,
            ConnectionState connectionState,
            String database,
            LoggedDbCommandExecutionType executionType,
            CommandType commandType,
            Int32 commandTimeout,
            String commandText,
            String parameters )
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
        public Int32 CommandId
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
        public String Database
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
        public Int32 CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}
