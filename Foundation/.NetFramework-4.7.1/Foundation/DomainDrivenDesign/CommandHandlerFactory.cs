using System;

namespace Foundation.DomainDrivenDesign
{
    public static class CommandHandlerFactory
    {
        public static IAsyncCommandHandler<TCommand, TCommandResult> Create<TCommand, TCommandResult>(Func<TCommand, TCommandResult> handle)
            where TCommand : ICommand where TCommandResult : ICommandResult
        {
            return new CommandHandler<TCommand, TCommandResult>(handle);
        }

        public static IAsyncCommandHandler<TCommand, TCommandResult> Create<TCommand, TCommandResult>(
            ICommandHandler<TCommand, TCommandResult> handler)
            where TCommand : ICommand where TCommandResult : ICommandResult
        {
            return new CommandHandler<TCommand, TCommandResult>(handler.Handle);
        }
    }
}