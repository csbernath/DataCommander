namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;

    /// <summary>
    /// Summary description for LoggedSqlCommandFilterRule.
    /// </summary>
    internal sealed class SimpleLoggedSqlCommandFilterRule
    {
        private readonly bool include;
        private readonly string userName;
        private readonly string hostName;
        private readonly string database;
        private readonly string commandText;

        public SimpleLoggedSqlCommandFilterRule(
            bool include,
            string userName,
            string hostName,
            string database,
            string commandText)
        {
            this.include = include;
            this.userName = userName;
            this.hostName = hostName;
            this.database = database;
            this.commandText = commandText;
        }

        public bool Match(
            string userName,
            string hostName,
            IDbCommand command)
        {
            string database = command.Connection.Database;
            string commandText = command.CommandText;
            bool match =
                (this.userName == null || this.userName == userName) &&
                (this.hostName == null || this.hostName == hostName) &&
                (this.database == null || this.database == database) &&
                (this.commandText == null || this.commandText == commandText);

            return match;
        }

        public bool Include
        {
            get
            {
                return this.include;
            }
        }
    }
}