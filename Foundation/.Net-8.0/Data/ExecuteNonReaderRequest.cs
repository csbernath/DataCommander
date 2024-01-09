using System.Collections.ObjectModel;
using System.Threading;

namespace Foundation.Data;

public sealed class ExecuteNonReaderRequest(CreateCommandRequest createCommandRequest, CancellationToken cancellationToken)
{
    public readonly CreateCommandRequest CreateCommandRequest = createCommandRequest;
    public readonly CancellationToken CancellationToken = cancellationToken;

    public ExecuteNonReaderRequest(string commandText)
        : this(new CreateCommandRequest(commandText), CancellationToken.None)
    {
    }

    public ExecuteNonReaderRequest(string commandText, ReadOnlyCollection<object> parameters)
        : this(new CreateCommandRequest(commandText, parameters), CancellationToken.None)
    {
    }
}