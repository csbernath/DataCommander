
namespace Foundation.Data.LoggedDbConnection
{
    public sealed class BeforeOpenDbConnectionEventArgs : LoggedEventArgs
    {
        public BeforeOpenDbConnectionEventArgs(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }
    }
}