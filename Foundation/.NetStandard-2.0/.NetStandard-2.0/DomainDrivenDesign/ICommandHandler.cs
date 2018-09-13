namespace Foundation.DomainDrivenDesign
{
    public interface ICommandHandler<in TCommand, out TCommandResult> where TCommand : ICommand where TCommandResult : ICommandResult
    {
        TCommandResult Handle(TCommand command);
    }
}