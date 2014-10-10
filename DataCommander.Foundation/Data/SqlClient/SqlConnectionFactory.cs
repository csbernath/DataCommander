namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    internal sealed class SqlConnectionFactory : IDbConnectionHelper
    {
        private readonly IDbConnection connection;

        public event EventHandler InfoMessage;

        public SqlConnectionFactory(
            SqlConnection sqlConnection,
            IDbConnection connection)
        {
            sqlConnection.InfoMessage += this.InfoMessageEvent;
            this.connection = connection;
        }

        private void InfoMessageEvent(Object sender, SqlInfoMessageEventArgs e)
        {
            if (this.InfoMessage != null)
            {
                this.InfoMessage(this.connection, e);
            }
        }
    }
}