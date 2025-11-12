using System.Text;
using DataCommander.Api;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.PostgreSql;

internal sealed class ObjectName(SqlObject sqlObject, string schemaName, string objectName) : IObjectName
{
    private readonly SqlObject _sqlObject = sqlObject;

    string IObjectName.UnquotedName
    {
        get
        {
            var stringBuilder = new StringBuilder();
            if (schemaName != null)
            {
                stringBuilder.Append(schemaName);
                stringBuilder.Append('.');
            }

            stringBuilder.Append(objectName);

            return stringBuilder.ToString();
        }
    }

    string IObjectName.QuotedName
    {
        get
        {
            var stringBuilder = new StringBuilder();
            var sqlCommandBuilder = new SqlCommandBuilder();

            if (schemaName != null)
            {
                stringBuilder.Append(QuoteIdentifier(schemaName));
                stringBuilder.Append('.');
            }
            //else if (this.sqlObject.ParentAlias != null)
            //{
            //    sb.Append(this.sqlObject.ParentAlias);
            //    sb.Append('.');
            //}

            stringBuilder.Append(QuoteIdentifier(objectName));

            return stringBuilder.ToString();
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