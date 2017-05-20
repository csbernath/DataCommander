namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;

    /// <summary>
    /// Summary description for LoggedSqlCommandFilterRule.
    /// </summary>
    internal sealed class SimpleLoggedSqlCommandFilterRule
    {
        private readonly string _userName;
        private readonly string _hostName;
        private readonly string _database;
        private readonly string _commandText;

        public SimpleLoggedSqlCommandFilterRule(
            bool include,
            string userName,
            string hostName,
            string database,
            string commandText)
        {
            this.Include = include;
            this._userName = userName;
            this._hostName = hostName;
            this._database = database;
            this._commandText = commandText;
        }

        public bool Match(
            string userName,
            string hostName,
            IDbCommand command)
        {
            var database = command.Connection.Database;
            var commandText = command.CommandText;
            var match =
                (this._userName == null || this._userName == userName) &&
                (this._hostName == null || this._hostName == hostName) &&
                (this._database == null || this._database == database) &&
                (this._commandText == null || this._commandText == commandText);

            return match;
        }

        public bool Include { get; }
    }
}