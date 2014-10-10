namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Security.Principal;
#if FOUNDATION_3_5
    using System.Web;
#endif
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SimpleSqlConnectionFactory
    {
        private String connectionString;
        private Int32 commandTimeout;
        private IDbConnectionFactory factory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="nodeName"></param>
        public SimpleSqlConnectionFactory( ConfigurationSection section, String nodeName )
        {
            Contract.Requires(section != null);

            ConfigurationNode node = section.SelectNode( nodeName, true );
            this.connectionString = node.Attributes["ConnectionString"].GetValue<String>();
            TimeSpan timeSpan;

            Boolean contains = node.Attributes.TryGetAttributeValue<TimeSpan>( "CommandTimeout", out timeSpan );

            if (contains)
            {
                this.commandTimeout = (Int32)timeSpan.TotalSeconds;
            }
            else
            {
                this.commandTimeout = 259200; // 3 days
            }

            Boolean isSafe;
            node.Attributes.TryGetAttributeValue<Boolean>( "IsSafe", out isSafe );
            ConfigurationNode sqlLogNode = node.ChildNodes["SqlLog"];
            Boolean enabled;
            sqlLogNode.Attributes.TryGetAttributeValue<Boolean>( "Enabled", out enabled );

            if (enabled)
            {
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder( this.connectionString );
                String applicationName = sqlConnectionStringBuilder.ApplicationName;
                String logConnectionString;
                contains = sqlLogNode.Attributes.TryGetAttributeValue<String>( "ConnectionString", null, out logConnectionString );

                if (!contains)
                {
                    String dataSource;
                    contains = sqlLogNode.Attributes.TryGetAttributeValue<String>( "Data Source", null, out dataSource );

                    if (!contains)
                    {
                        dataSource = sqlConnectionStringBuilder.DataSource;
                    }

                    String initialCatalog = sqlLogNode.Attributes["Initial Catalog"].GetValue<String>();

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

                String loggedSqlCommandFilterNodeName = sqlLogNode.FullName + ConfigurationNode.Delimiter + "LoggedSqlCommandFilter";
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
        public Int32 CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnectionFactory Factory
        {
            get
            {
                return this.factory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public System.Data.IDbConnection CreateConnection( String name )
        {
            String userName = null;
            String hostName = null;
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

            String connectionString;

            if (name != null)
            {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder( this.connectionString );
                sqlConnectionStringBuilder.ApplicationName = String.Format( CultureInfo.InvariantCulture, "{0} {1}", sqlConnectionStringBuilder.ApplicationName, name );
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