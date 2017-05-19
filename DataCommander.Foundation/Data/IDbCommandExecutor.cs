namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using System.Data.Common;
    using System.Threading;

    public interface IDbCommandExecutor
    {
        void Execute(Action<IDbConnection> execute);
        Task ExecuteAsync(Action<DbConnection> execute, CancellationToken cancellationToken);
    }
}