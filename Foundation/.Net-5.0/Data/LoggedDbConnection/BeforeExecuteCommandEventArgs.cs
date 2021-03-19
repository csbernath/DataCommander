
namespace Foundation.Data.LoggedDbConnection
{
    public sealed class BeforeExecuteCommandEventArgs : LoggedEventArgs
    {
        public BeforeExecuteCommandEventArgs(LoggedDbCommandInfo command) => Command = command;

        public LoggedDbCommandInfo Command { get; }
    }
}