namespace DataCommander.Providers.PostgreSql
{
    using System.Data.SqlClient;
    using System.Text;

    internal sealed class ObjectName : IObjectName
    {
        private SqlObject sqlObject;
        private readonly string schemaName;
        private readonly string objectName;

        public ObjectName(SqlObject sqlObject, string schemaName, string objectName)
        {
            this.sqlObject = sqlObject;
            this.schemaName = schemaName;
            this.objectName = objectName;
        }

        string IObjectName.UnquotedName
        {
            get
            {
                var sb = new StringBuilder();
                if (schemaName != null)
                {
                    sb.Append(schemaName);
                    sb.Append('.');
                }

                sb.Append(objectName);

                return sb.ToString();
            }
        }

        string IObjectName.QuotedName
        {
            get
            {
                var sb = new StringBuilder();
                var sqlCommandBuilder = new SqlCommandBuilder();

                if (schemaName != null)
                {
                    sb.Append(QuoteIdentifier(schemaName));
                    sb.Append('.');
                }
                //else if (this.sqlObject.ParentAlias != null)
                //{
                //    sb.Append(this.sqlObject.ParentAlias);
                //    sb.Append('.');
                //}

                sb.Append(QuoteIdentifier(objectName));

                return sb.ToString();
            }
        }

        private static string QuoteIdentifier(string unquotedIdentifier)
        {
            string quotedIdentifier;

            if (unquotedIdentifier.IndexOfAny(new[] {'.', '-'}) >= 0)
            {
                quotedIdentifier = new SqlCommandBuilder().QuoteIdentifier(unquotedIdentifier);
            }
            else
            {
                quotedIdentifier = unquotedIdentifier;
            }

            return quotedIdentifier;
        }
    }
}