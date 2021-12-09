using Microsoft.Data.SqlClient;
using System.Text;
using DataCommander.Providers2;

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
                var stringBuilder = new StringBuilder();
                if (_schemaName != null)
                {
                    stringBuilder.Append(QuoteIdentifier(_schemaName));
                    stringBuilder.Append('.');
                }

                stringBuilder.Append(QuoteIdentifier(_objectName));
                return stringBuilder.ToString();
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