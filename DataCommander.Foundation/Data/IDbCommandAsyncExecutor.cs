using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Foundation.Data
{
    public interface IDbCommandAsyncExecutor : IDbCommandExecutor
    {
        Task ExecuteAsync(Func<DbConnection, Task> execute, CancellationToken cancellationToken);
    }
}