using System;
using System.Data;
using Foundation.Assertions;

namespace Foundation.Data
{
    internal sealed class DbCommandExecutor : IDbCommandExecutor
    {
        private readonly IDbConnection _connection;

        public DbCommandExecutor(IDbConnection connection)
        {
            Assert.IsNotNull(connection);
            _connection = connection;
        }

        public void Execute(Action<IDbConnection> execute)
        {
            Assert.IsNotNull(execute);
            execute(_connection);
        }
    }
}