using System;
using System.Data;

namespace Foundation.Data
{
    internal sealed class DbCommandExecutor : IDbCommandExecutor
    {
        private readonly IDbConnection _connection;
        public DbCommandExecutor(IDbConnection connection) => _connection = connection;
        public void Execute(Action<IDbConnection> execute) => execute(_connection);
    }
}