namespace DataCommander.Providers.SQLite
{
    using System.Data.SQLite;
    using Providers.Connection;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SQLiteConnectionStringBuilder sqLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => sqLiteConnectionStringBuilder.ConnectionString;

            set => sqLiteConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return true;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            sqLiteConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            bool contains;
            switch (keyword)
            {
                case ConnectionStringKeyword.IntegratedSecurity:
                    contains = sqLiteConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out value);
                    if (contains)
                    {
                        value = bool.Parse((string) value);
                    }
                    break;

                default:
                    contains = sqLiteConnectionStringBuilder.TryGetValue(keyword, out value);
                    break;
            }
            return contains;
        }
    }
}