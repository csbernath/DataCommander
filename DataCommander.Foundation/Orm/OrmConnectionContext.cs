namespace DataCommander.Foundation.Orm
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Diagnostics.Contracts;

    internal sealed class OrmConnectionContext : IDisposable
    {
        private readonly Func<DbConnection> _connectionFactory;
        private DbConnection _connection;
        private bool _disposable;

        public OrmConnectionContext(Func<DbConnection> connectionFactory)
        {
            FoundationContract.Requires<ArgumentNullException>(connectionFactory != null);
            _connectionFactory = connectionFactory;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            _connection = _connectionFactory();

            if (_connection.State == ConnectionState.Closed)
            {
                _disposable = true;
                await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public DbConnection Connection => _connection;

        void IDisposable.Dispose()
        {
            if (_disposable)
                _connection.Dispose();
        }
    }
}