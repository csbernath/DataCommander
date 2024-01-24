using System;
using System.Collections.ObjectModel;
using System.Data;

namespace Foundation.Data;

public sealed class ExecuteReaderRequest
{
    public readonly CreateCommandRequest CreateCommandRequest;
    public readonly CommandBehavior CommandBehavior;

    public ExecuteReaderRequest(CreateCommandRequest createCommandRequest, CommandBehavior commandBehavior)
    {
        ArgumentNullException.ThrowIfNull(createCommandRequest);

        CreateCommandRequest = createCommandRequest;
        CommandBehavior = commandBehavior;
    }

    public ExecuteReaderRequest(CreateCommandRequest createCommandRequest)
        : this(createCommandRequest, CommandBehavior.Default)
    {
    }

    public ExecuteReaderRequest(string commandText, ReadOnlyCollection<object> parameters, IDbTransaction transaction)
        : this(new CreateCommandRequest(commandText, parameters, CommandType.Text, null, transaction), CommandBehavior.Default)
    {
    }

    public ExecuteReaderRequest(string commandText, ReadOnlyCollection<object> parameters)
        : this(commandText, parameters, null)
    {
    }

    public ExecuteReaderRequest(string commandText)
        : this(commandText, null)
    {
    }
}