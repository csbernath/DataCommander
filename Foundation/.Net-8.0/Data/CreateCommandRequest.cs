using System.Collections.ObjectModel;
using System.Data;

namespace Foundation.Data;

public class CreateCommandRequest(
    string commandText,
    ReadOnlyCollection<object> parameters,
    CommandType commandType,
    int? commandTimeout,
    IDbTransaction transaction)
{
    public readonly string CommandText = commandText;
    public readonly ReadOnlyCollection<object> Parameters = parameters;
    public readonly CommandType CommandType = commandType;
    public readonly int? CommandTimeout = commandTimeout;
    public readonly IDbTransaction Transaction = transaction;

    public CreateCommandRequest(string commandText, ReadOnlyCollection<object> parameters)
        : this(commandText, parameters, CommandType.Text, null, null)
    {
    }

    public CreateCommandRequest(string commandText)
        : this(commandText, null)
    {
    }
}