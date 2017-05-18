namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using Orm;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ExecuteReaderRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createCommandRequest"></param>
        /// <param name="commandBehavior"></param>
        /// <param name="cancellationToken"></param>
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

        public ExecuteReaderRequest(string commandText, IEnumerable<object> parameters)
            : this(new CreateCommandRequest(commandText, parameters), CommandBehavior.Default, CancellationToken.None)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly CreateCommandRequest CreateCommandRequest;

        /// <summary>
        /// 
        /// </summary>
        public readonly CommandBehavior CommandBehavior;

        /// <summary>
        /// 
        /// </summary>
        public readonly CancellationToken CancellationToken;
    }
}