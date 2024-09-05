
namespace Foundation.Data.LoggedDbConnection;

public sealed class BeforeExecuteCommandEventArgs(LoggedDbCommandInfo command) : LoggedEventArgs
{
    public LoggedDbCommandInfo Command { get; } = command;
}