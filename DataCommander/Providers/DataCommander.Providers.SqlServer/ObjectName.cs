using System.Data.SqlClient;
using System.Text;

namespace DataCommander.Providers.SqlServer
{
    internal sealed class ObjectName : IObjectName
    {
        private readonly string _objectName;
        private readonly string _schemaName;

        public ObjectName(string schemaName, string objectName)
        {
            _schemaName = schemaName;
            _objectName = objectName;
        }

        string IObjectName.UnquotedName
        {
            get
            {
                var sb = new StringBuilder();
                if (_schemaName != null)
                {
                    sb.Append(_schemaName);
                    sb.Append('.');
                }

                sb.Append(_objectName);

                return sb.ToString();
            }
        }

        string IObjectName.QuotedName
        {
            get
            {
                var sb = new StringBuilder();
                var sqlCommandBuilder = new SqlCommandBuilder();

                if (_schemaName != null)
                {
                    sb.Append(QuoteIdentifier(_schemaName));
                    sb.Append('.');
                }

                sb.Append(QuoteIdentifier(_objectName));

                return sb.ToString();
            }
        }

        private static string QuoteIdentifier(string unquotedIdentifier)
        {
            var quotedIdentifier = unquotedIdentifier.IndexOfAny(new[] {'.', '-'}) >= 0
                ? new SqlCommandBuilder().QuoteIdentifier(unquotedIdentifier)
                : unquotedIdentifier;

            return quotedIdentifier;
        }
    }
}