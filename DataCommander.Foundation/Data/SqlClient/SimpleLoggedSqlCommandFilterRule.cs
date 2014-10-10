namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;

    /// <summary>
    /// Summary description for LoggedSqlCommandFilterRule.
    /// </summary>
    internal sealed class SimpleLoggedSqlCommandFilterRule
    {
        private Boolean include;
        private readonly String userName;
        private String hostName;
        private readonly String database;
        private readonly String commandText;

        public SimpleLoggedSqlCommandFilterRule(
            Boolean include,
            String userName,
            String hostName,
            String database,
            String commandText)
        {
            this.include = include;
            this.userName = userName;
            this.hostName = hostName;
            this.database = database;
            this.commandText = commandText;
        }

        public Boolean Match(
            String userName,
            String hostName,
            IDbCommand command)
        {
            String database = command.Connection.Database;
            String commandText = command.CommandText;
            Boolean match =
                (this.userName == null || this.userName == userName) &&
                (this.hostName == null || this.hostName == hostName) &&
                (this.database == null || this.database == database) &&
                (this.commandText == null || this.commandText == commandText);

            return match;
        }

        public Boolean Include
        {
            get
            {
                return this.include;
            }
        }
    }
}