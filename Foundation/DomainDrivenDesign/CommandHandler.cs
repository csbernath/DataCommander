using System;
using System.Threading.Tasks;

namespace Foundation.DomainDrivenDesign
{
    internal sealed class CommandHandler<TCommand, TCommandResult> : IAsyncCommandHandler<TCommand, TCommandResult>
        where TCommand : ICommand where TCommandResult : ICommandResult
    {
        private readonly Func<TCommand, TCommandResult> _handle;

        public CommandHandler(Func<TCommand, TCommandResult> handle)
        {
            _handle = handle;
        }

        public async Task<TCommandResult> HandleAsync(TCommand command)
        {
            return await Task.Run(() => _handle(command));
        }
    }
}