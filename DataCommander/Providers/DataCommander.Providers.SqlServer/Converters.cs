using System;
using DataCommander.Api.FieldReaders;
using DataCommander.Api.Query;
using Foundation.Core;

namespace DataCommander.Providers.SqlServer;

internal static class Converters
{
    public static object ConvertToDecimal(object source)
    {
        object target;
        if (source == DBNull.Value)
            target = DBNull.Value;
        else
        {
            var decimalField = (DecimalField)source;
            target = decimalField.DecimalValue;
        }

        return target;
    }

    public static object ConvertToString(object? source)
    {
        object target;
        if (source == null || source == DBNull.Value)
            target = DBNull.Value;
        else
        {
            var convertible = (IConvertible)source;
            target = convertible.ToString(null);
        }

        return target;
    }

    public static bool IsBatchSeparator(ReadOnlySpan<char> commandText, Token token)
    {
        const char newLine = '\n';
        const string batchSeparator = "GO";

        var isBatchSeparator =
            token.Type == TokenType.KeyWord &&
            string.Compare(token.Value, batchSeparator, StringComparison.InvariantCultureIgnoreCase) == 0;

        if (isBatchSeparator)
        {
            var lineStartIndex = commandText.LastIndexOf(newLine, token.StartPosition);
            lineStartIndex++;
            var lineEndIndex = commandText.IndexOf(newLine, token.EndPosition + 1);
            if (lineEndIndex == -1)
                lineEndIndex = commandText.Length - 1;
            var lineLength = lineEndIndex - lineStartIndex + 1;
            var line = commandText.Slice(lineStartIndex, lineLength);
            line = line.Trim();
            isBatchSeparator = line.CompareTo(batchSeparator, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        return isBatchSeparator;
    }

    public static Exception UnAggregateException(AggregateException aggregateException)
    {
        Exception unaggregatedException;
        if (aggregateException.InnerExceptions.Count == 1)
        {
            var innerException = aggregateException.InnerExceptions[0];
            if (innerException is AggregateException aggregateException2)
                unaggregatedException = UnAggregateException(aggregateException2);
            else
                unaggregatedException = innerException;
        }
        else
            unaggregatedException = aggregateException;

        return unaggregatedException;
    }
}