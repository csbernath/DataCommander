namespace DataCommander.Providers.Wmi
{
    using System.Data;
    using System.Data.Common;
    using System.Management;

    internal sealed class WmiConnection : IDbConnection
    {
        private readonly string connectionString;
        private string dataSource;
        private ManagementScope scope;

        public WmiConnection( string connectionString )
        {
            this.connectionString = connectionString;
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
            DbConnectionStringBuilder sb = new DbConnectionStringBuilder();
            sb.ConnectionString = this.connectionString;
            object value;            
            bool contains = sb.TryGetValue( ConnectionStringKeyword.DataSource, out value );
            string dataSource;

            if (contains)
            {
                dataSource = (string) value;
            }
            else
            {
                dataSource = null;
            }

            this.dataSource = dataSource;
            //ManagementPath path = ManagementPath.DefaultPath;
            //path.Server = dataSource;
            //scope = new ManagementScope( path );
            
            //string path = @"\\binarit-srv06\root\virtualization";
            string path = this.dataSource;
            this.scope = new ManagementScope( path );
        }

        public string ConnectionString
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        public int ConnectionTimeout => 0;

        public string Database => null;

        public ConnectionState State => ConnectionState.Closed;

        public ManagementScope Scope => this.scope;

        public string DataSource => this.dataSource;
    }
}