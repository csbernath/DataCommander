namespace DataCommander.Providers.OleDb
{
    using System.Data.OleDb;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new OleDbConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => this.oleDbConnectionStringBuilder.ConnectionString;

            set => this.oleDbConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return true;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            this.oleDbConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return this.oleDbConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}