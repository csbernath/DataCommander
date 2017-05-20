namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;

    public sealed class ExecuteReaderRequest
    {
        public ExecuteReaderRequest(CreateCommandRequest initializeCommandRequest, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            InitializeCommandRequest = initializeCommandRequest;
            CommandBehavior = commandBehavior;
            CancellationToken = cancellationToken;
        }

        public ExecuteReaderRequest(string commandText)
            : this(new CreateCommandRequest(commandText), CommandBehavior.Default, CancellationToken.None)
        {
        }

        public ExecuteReaderRequest(string commandText, IEnumerable<object> parameters)
            : this(new CreateCommandRequest(commandText, parameters), CommandBehavior.Default, CancellationToken.None)
        {
        }

        public readonly CreateCommandRequest InitializeCommandRequest;
        public readonly CommandBehavior CommandBehavior;
        public readonly CancellationToken CancellationToken;
    }
}