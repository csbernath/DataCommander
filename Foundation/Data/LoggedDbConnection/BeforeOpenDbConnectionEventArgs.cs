
namespace Foundation.Data.LoggedDbConnection;

public sealed class BeforeOpenDbConnectionEventArgs(string connectionString) : LoggedEventArgs
{
    public string ConnectionString { get; } = connectionString;
}