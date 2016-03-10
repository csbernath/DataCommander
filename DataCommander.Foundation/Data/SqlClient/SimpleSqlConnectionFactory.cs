namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Security.Principal;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Threading;

#if FOUNDATION_3_5
    using System.Web;
#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class SimpleSqlConnectionFactory
    {
        private readonly string connectionString;
        private readonly int commandTimeout;
        private readonly IDbConnectionFactory factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="nodeName"></param>
        public SimpleSqlConnectionFactory( ConfigurationSection section, string nodeName )
        {
            Contract.Requires(section != null);

            ConfigurationNode node = section.SelectNode( nodeName, true );
            this.connectionString = node.Attributes["ConnectionString"].GetValue<string>();
            TimeSpan timeSpan;

            bool contains = node.Attributes.TryGetAttributeValue( "CommandTimeout", out timeSpan );

            if (contains)
            {
                this.commandTimeout = (int)timeSpan.TotalSeconds;
            }
            else
            {
                this.commandTimeout = 259200; // 3 days
            }

            bool isSafe;
            node.Attributes.TryGetAttributeValue( "IsSafe", out isSafe );
            ConfigurationNode sqlLogNode = node.ChildNodes["SqlLog"];
            bool enabled;
            sqlLogNode.Attributes.TryGetAttributeValue( "Enabled", out enabled );

            if (enabled)
            {
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder( this.connectionString );
                string applicationName = sqlConnectionStringBuilder.ApplicationName;
                string logConnectionString;
                contains = sqlLogNode.Attributes.TryGetAttributeValue( "ConnectionString", null, out logConnectionString );

                if (!contains)
                {
                    string dataSource;
                    contains = sqlLogNode.Attributes.TryGetAttributeValue( "Data Source", null, out dataSource );

                    if (!contains)
                    {
                        dataSource = sqlConnectionStringBuilder.DataSource;
                    }

                    string initialCatalog = sqlLogNode.Attributes["Initial Catalog"].GetValue<string>();

                    sqlConnectionStringBuilder =
                        new SqlConnectionStringBuilder
                        {
                            DataSource = dataSource,
                            InitialCatalog = initialCatalog,
                            IntegratedSecurity = true,
                            ApplicationName = applicationName + " (SqlLog)",
                            Pooling = false
                        };

                    logConnectionString = sqlConnectionStringBuilder.ConnectionString;
                }

                string loggedSqlCommandFilterNodeName = sqlLogNode.FullName + ConfigurationNode.Delimiter + "LoggedSqlCommandFilter";
                SimpleLoggedSqlCommandFilter filter = new SimpleLoggedSqlCommandFilter( section, loggedSqlCommandFilterNodeName );

                if (isSafe)
                {
                    this.factory = new SafeLoggedSqlConnectionFactory( logConnectionString, applicationName, filter );
                }
                else
                {
                    this.factory = new SqlLoggedSqlConnectionFactory( logConnectionString, applicationName, filter );
                }

                WorkerThread thread = this.factory.Thread;
                thread.Start();
            }
            else
            {
                if (isSafe)
                {
                    this.factory = new SafeSqlConnectionFactory();
                }
                else
                {
                    this.factory = new NativeSqlCommandFactory();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout => this.commandTimeout;

        /// <summary>
        /// 
        /// </summary>
        public IDbConnectionFactory Factory => this.factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection( string name )
        {
            string userName = null;
            string hostName = null;
#if FOUNDATION_3_5
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                HttpRequest request = context.Request;

                if (request != null)
                {
                    userName = request.ServerVariables["AUTH_USER"];

                    if (userName.Length == 0)
                    {
                        userName = null;
                    }

                    hostName = request.UserHostName;

                    if (hostName.Length == 0)
                    {
                        hostName = request.UserHostAddress;
                    }

                    if (hostName == "127.0.0.1")
                    {
                        hostName = Environment.MachineName;
                    }
                }
            }
            else
            {
                userName = WindowsIdentity.GetCurrent().Name;
                hostName = Environment.MachineName;
            }
#else
            userName = WindowsIdentity.GetCurrent().Name;
            hostName = Environment.MachineName;
#endif

            string connectionString;

            if (name != null)
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder( this.connectionString );
                sqlConnectionStringBuilder.ApplicationName = string.Format( CultureInfo.InvariantCulture, "{0} {1}", sqlConnectionStringBuilder.ApplicationName, name );
                connectionString = sqlConnectionStringBuilder.ConnectionString;
            }
            else
            {
                connectionString = this.connectionString;
            }

            return this.factory.CreateConnection( connectionString, userName, hostName );
        }
    }
}