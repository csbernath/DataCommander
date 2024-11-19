using System.Collections.Generic;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data;

public sealed class ExecuteReaderRequest
{
    public readonly CreateCommandRequest CreateCommandRequest;
    public readonly CommandBehavior CommandBehavior;

    public ExecuteReaderRequest(CreateCommandRequest createCommandRequest, CommandBehavior commandBehavior)
    {
        Assert.IsNotNull(createCommandRequest);
        CreateCommandRequest = createCommandRequest;
        CommandBehavior = commandBehavior;
    }

    public ExecuteReaderRequest(CreateCommandRequest createCommandRequest)
        : this(createCommandRequest, CommandBehavior.Default)
    {
    }

    public ExecuteReaderRequest(string commandText, IReadOnlyCollection<object>? parameters, IDbTransaction? transaction)
        : this(new CreateCommandRequest(commandText, parameters, CommandType.Text, null, transaction), CommandBehavior.Default)
    {
    }

    public ExecuteReaderRequest(string commandText, IReadOnlyCollection<object>? parameters)
        : this(commandText, parameters, null)
    {
    }

    public ExecuteReaderRequest(string commandText)
        : this(commandText, null)
    {
    }
}