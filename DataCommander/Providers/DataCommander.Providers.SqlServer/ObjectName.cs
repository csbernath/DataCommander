using System;
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

    private static string QuoteIdentifier(string unquotedIdentifier)
    {
        var quotedIdentifier = unquotedIdentifier.IndexOfAny(['.', '-']) >= 0 || IsKeyWord(unquotedIdentifier) 
            ? new SqlCommandBuilder().QuoteIdentifier(unquotedIdentifier)
            : unquotedIdentifier;

        return quotedIdentifier;
    }

    private static bool IsKeyWord(string unquotedIdentifier)
    {
        var keywords = KeyWordRepository.Get();
        var index = Array.BinarySearch(keywords, unquotedIdentifier, StringComparer.InvariantCultureIgnoreCase);
        var isKeyWord = index >= 0;
        return isKeyWord;
    }
}