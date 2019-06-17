using System.Collections.ObjectModel;
using System.Threading;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data
{
    public sealed class ExecuteNonReaderRequest
    {
        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly CancellationToken CancellationToken;

        public ExecuteNonReaderRequest(CreateCommandRequest createCommandRequest, CancellationToken cancellationToken)
        {
            CreateCommandRequest = createCommandRequest;
            CancellationToken = cancellationToken;
        }

        public ExecuteNonReaderRequest(string commandText)
            : this(new CreateCommandRequest(commandText), CancellationToken.None)
        {
        }

        public ExecuteNonReaderRequest(string commandText, ReadOnlyCollection<object> parameters)
            : this(new CreateCommandRequest(commandText, parameters), CancellationToken.None)
        {
        }
    }
}