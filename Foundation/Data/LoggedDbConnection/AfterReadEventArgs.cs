
namespace Foundation.Data.LoggedDbConnection;

public sealed class AfterReadEventArgs : LoggedEventArgs
{
    public AfterReadEventArgs(int rowCount) => RowCount = rowCount;

    public int RowCount { get; }
}