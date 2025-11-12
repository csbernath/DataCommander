using Microsoft.Data.SqlClient;
using System.Text;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer;

internal sealed class ObjectName(string? schemaName, string objectName) : IObjectName
{
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
            if (schemaName != null)
            {
                stringBuilder.Append(QuoteIdentifier(schemaName));
                stringBuilder.Append('.');
            }

            stringBuilder.Append(QuoteIdentifier(objectName));
            return stringBuilder.ToString();
        }
    }

    public static string QuoteIdentifier(string unquotedIdentifier)
    {
        var quotedIdentifier = unquotedIdentifier.IndexOfAny(['.', '-']) >= 0 || IsKeyWord(unquotedIdentifier) 
            ? new SqlCommandBuilder().QuoteIdentifier(unquotedIdentifier)
            : unquotedIdentifier;

        return quotedIdentifier;
    }

    private static bool IsKeyWord(string unquotedIdentifier)
    {
        var keywords = KeyWordRepository.Get();
        return keywords.Contains(unquotedIdentifier);
    }
}