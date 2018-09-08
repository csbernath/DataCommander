using System.Data;
using Foundation.Threading;

namespace Foundation.Data
{
    public interface IDbConnectionFactory
    {
        WorkerThread Thread { get; }
        IDbConnection CreateConnection(string connectionString, string userName, string hostName);
        IDbConnectionHelper CreateConnectionHelper(IDbConnection connection);
    }
}