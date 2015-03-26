namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Text;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class DbCommandExecutionException : Exception
    {
        private readonly string database;
        private readonly string commandText;
        private readonly int commandTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="command"></param>
        public DbCommandExecutionException(string message, Exception innerException, IDbCommand command)
            : base(message, innerException)
        {
            this.commandText = command.ToLogString();
            this.commandTimeout = command.CommandTimeout;
            this.database = command.Connection.Database;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DbCommandExecutionException: {0}\r\ninnerException:\r\n{1}\r\ndatabase: {2}\r\ncommandTimeout: {3}\r\ncommandText: {4}",
                this.Message,
                this.InnerException.ToLogString(),
                this.database,
                this.commandTimeout,
                this.commandText);
            string s = sb.ToString();
            return s;
        }
    }
}