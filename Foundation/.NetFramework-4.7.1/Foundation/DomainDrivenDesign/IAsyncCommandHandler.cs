namespace Foundation.DomainDrivenDesign
{
    public interface IAsyncCommandHandler<in TCommand, TCommandResult> : IAsyncMessageHandler<TCommand, TCommandResult>
        where TCommand : ICommand where TCommandResult : ICommandResult
    {
    }
}