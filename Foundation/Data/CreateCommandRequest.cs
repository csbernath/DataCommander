using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public class CreateCommandRequest(
    string commandText,
    IReadOnlyCollection<object>? parameters,
    CommandType commandType,
    int? commandTimeout,
    IDbTransaction? transaction)
{
    public readonly string CommandText = commandText;
    public readonly IReadOnlyCollection<object>? Parameters = parameters;
    public readonly CommandType CommandType = commandType;
    public readonly int? CommandTimeout = commandTimeout;
    public readonly IDbTransaction? Transaction = transaction;

    public CreateCommandRequest(string commandText, IReadOnlyCollection<object>? parameters)
        : this(commandText, parameters, CommandType.Text, null, null)
    {
    }

    public CreateCommandRequest(string commandText)
        : this(commandText, null)
    {
    }
}