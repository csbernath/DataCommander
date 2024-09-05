
namespace Foundation.Data.LoggedDbConnection;

public sealed class AfterReadEventArgs(int rowCount) : LoggedEventArgs
{
    public int RowCount { get; } = rowCount;
}