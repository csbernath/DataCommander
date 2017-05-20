namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    internal sealed class SqlConnectionFactory : IDbConnectionHelper
    {
        private readonly IDbConnection _connection;

        public event EventHandler InfoMessage;

        public SqlConnectionFactory(
            SqlConnection sqlConnection,
            IDbConnection connection)
        {
            sqlConnection.InfoMessage += this.InfoMessageEvent;
            this._connection = connection;
        }

        private void InfoMessageEvent(object sender, SqlInfoMessageEventArgs e)
        {
            this.InfoMessage?.Invoke(this._connection, e);
        }
    }
}