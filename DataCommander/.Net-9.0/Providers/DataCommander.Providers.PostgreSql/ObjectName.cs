using System.Text;
using DataCommander.Api;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.PostgreSql;

internal sealed class ObjectName(SqlObject sqlObject, string schemaName, string objectName) : IObjectName
{
    private readonly SqlObject _sqlObject = sqlObject;
    private readonly string _schemaName = schemaName;
    private readonly string _objectName = objectName;

    string IObjectName.UnquotedName
    {
        get
        {
            StringBuilder sb = new StringBuilder();
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
            StringBuilder sb = new StringBuilder();
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder();

            if (_schemaName != null)
            {
                sb.Append(QuoteIdentifier(_schemaName));
                sb.Append('.');
            }
            //else if (this.sqlObject.ParentAlias != null)
            //{
            //    sb.Append(this.sqlObject.ParentAlias);
            //    sb.Append('.');
            //}

            sb.Append(QuoteIdentifier(_objectName));

            return sb.ToString();
        }
    }

    private static string QuoteIdentifier(string unquotedIdentifier)
    {
        string quotedIdentifier;

        if (unquotedIdentifier.IndexOfAny(['.', '-']) >= 0)
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