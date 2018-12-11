using System.Data;
using System.Data.Common;
using System.Management;
using DataCommander.Providers.Connection;

namespace DataCommander.Providers.Wmi
{
    internal sealed class WmiConnection : IDbConnection
    {
        private readonly string _connectionString;

        public WmiConnection( string connectionString )
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
        }

        public IDbTransaction BeginTransaction()
        {
            return null;
        }

        public IDbTransaction BeginTransaction( IsolationLevel il )
        {
            return null;
        }

        public void ChangeDatabase( string databaseName )
        {
        }

        public void Close()
        {
        }

        public IDbCommand CreateCommand()
        {
            return new WmiCommand( this );
        }

        public void Open()
        {
            var sb = new DbConnectionStringBuilder();
            sb.ConnectionString = _connectionString;
            object value;            
            var contains = sb.TryGetValue( ConnectionStringKeyword.DataSource, out value );
            string dataSource;

            if (contains)
            {
                dataSource = (string) value;
            }
            else
            {
                dataSource = null;
            }

            DataSource = dataSource;
            //ManagementPath path = ManagementPath.DefaultPath;
            //path.Server = dataSource;
            //scope = new ManagementScope( path );

            var path = DataSource;
            Scope = new ManagementScope( path );
        }

        public string ConnectionString
        {
            get => null;

            set
            {
            }
        }

        public int ConnectionTimeout => 0;

        public string Database => null;

        public ConnectionState State => ConnectionState.Closed;

        public ManagementScope Scope { get; private set; }

        public string DataSource { get; private set; }
    }
}