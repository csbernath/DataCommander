namespace DataCommander.Foundation.Data
{
    using System;
    using System.Threading.Tasks;
    using System.Data.Common;
    using System.Threading;

    public interface IDbContext
    {
        Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken);
    }
}