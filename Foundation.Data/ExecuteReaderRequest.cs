using System.Data;
using System.Threading;
using Foundation.Assertions;
using Foundation.Collections;

namespace Foundation.Data
{
    public sealed class ExecuteReaderRequest
    {
        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly CommandBehavior CommandBehavior;
        public readonly CancellationToken CancellationToken;

        public ExecuteReaderRequest(CreateCommandRequest createCommandRequest, CommandBehavior commandBehavior, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(createCommandRequest);

            CreateCommandRequest = createCommandRequest;
            CommandBehavior = commandBehavior;
            CancellationToken = cancellationToken;
        }

        public ExecuteReaderRequest(CreateCommandRequest createCommandRequest)
            : this(createCommandRequest, CommandBehavior.Default, CancellationToken.None)
        {
        }

        public ExecuteReaderRequest(string commandText, ReadOnlyList<object> parameters, IDbTransaction transaction)
            : this(new CreateCommandRequest(commandText, parameters, CommandType.Text, null, transaction), CommandBehavior.Default, CancellationToken.None)
        {
        }

        public ExecuteReaderRequest(string commandText, ReadOnlyList<object> parameters)
            : this(commandText, parameters, null)
        {
        }

        public ExecuteReaderRequest(string commandText)
            : this(commandText, null)
        {
        }
    }
}