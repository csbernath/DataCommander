using System.Data;
using System.Threading;

namespace DataCommander.Foundation.Data
{
    public sealed class ExecuteReaderRequest
    {
        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly CommandBehavior CommandBehavior;
        public readonly CancellationToken CancellationToken;

        public ExecuteReaderRequest(CreateCommandRequest createCommandRequest, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            CreateCommandRequest = createCommandRequest;
            CommandBehavior = commandBehavior;
            CancellationToken = cancellationToken;
        }

        public ExecuteReaderRequest(string commandText)
            : this(new CreateCommandRequest(commandText), CommandBehavior.Default, CancellationToken.None)
        {
        }
    }
}