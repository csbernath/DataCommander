namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;

    public sealed class ExecuteReaderRequest
    {
        public ExecuteReaderRequest(InitializeCommandRequest initializeCommandRequest, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            InitializeCommandRequest = initializeCommandRequest;
            CommandBehavior = commandBehavior;
            CancellationToken = cancellationToken;
        }

        public ExecuteReaderRequest(string commandText)
            : this(new InitializeCommandRequest(commandText), CommandBehavior.Default, CancellationToken.None)
        {
        }

        public ExecuteReaderRequest(string commandText, IEnumerable<object> parameters)
            : this(new InitializeCommandRequest(commandText, parameters), CommandBehavior.Default, CancellationToken.None)
        {
        }

        public readonly InitializeCommandRequest InitializeCommandRequest;
        public readonly CommandBehavior CommandBehavior;
        public readonly CancellationToken CancellationToken;
    }
}