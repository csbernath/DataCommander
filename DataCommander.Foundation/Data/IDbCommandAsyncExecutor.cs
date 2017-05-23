using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Foundation.Data
{
    public interface IDbCommandAsyncExecutor : IDbCommandExecutor
    {
        Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken);
    }
}