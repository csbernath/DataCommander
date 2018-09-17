using System;

namespace DataCommander.Providers.OracleClient
{
    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        string IDbConnectionStringBuilder.ConnectionString
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => throw new NotImplementedException();
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => throw new NotImplementedException();
        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => throw new NotImplementedException();
    }
}