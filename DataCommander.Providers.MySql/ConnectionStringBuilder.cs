namespace DataCommander.Providers.MySql
{
    using System;
    using global::MySql.Data.MySqlClient;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => mySqlConnectionStringBuilder.ConnectionString;

            set => mySqlConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            throw new NotImplementedException();
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            throw new NotImplementedException();
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return mySqlConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}