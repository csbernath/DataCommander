using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Principal;
using Foundation.Configuration;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;

namespace Foundation.Data.SqlClient
{
#if FOUNDATION_3_5
    using System.Web;
#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class SimpleSqlConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="nodeName"></param>
        public SimpleSqlConnectionFactory(ConfigurationSection section, string nodeName)
        {
#if CONTRACTS_FULL
            Contract.Requires(section != null);
#endif

            var node = section.SelectNode(nodeName, true);
            this._connectionString = node.Attributes["ConnectionString"].GetValue<string>();
            TimeSpan timeSpan;

            var contains = node.Attributes.TryGetAttributeValue("CommandTimeout", out timeSpan);

            if (contains)
                this.CommandTimeout = (int)timeSpan.TotalSeconds;
            else
                this.CommandTimeout = 259200; // 3 days

            bool isSafe;
            node.Attributes.TryGetAttributeValue("IsSafe", out isSafe);
            var sqlLogNode = node.ChildNodes["SqlLog"];
            bool enabled;
            sqlLogNode.Attributes.TryGetAttributeValue("Enabled", out enabled);

            if (enabled)
            {
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(this._connectionString);
                var applicationName = sqlConnectionStringBuilder.ApplicationName;
                string logConnectionString;
                contains = sqlLogNode.Attributes.TryGetAttributeValue("ConnectionString", null, out logConnectionString);

                if (!contains)
                {
                    string dataSource;
                    contains = sqlLogNode.Attributes.TryGetAttributeValue("Data Source", null, out dataSource);

                    if (!contains)
                        dataSource = sqlConnectionStringBuilder.DataSource;

                    var initialCatalog = sqlLogNode.Attributes["Initial Catalog"].GetValue<string>();

                    sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                    {
                        DataSource = dataSource,
                        InitialCatalog = initialCatalog,
                        IntegratedSecurity = true,
                        ApplicationName = applicationName + " (SqlLog)",
                        Pooling = false
                    };

                    logConnectionString = sqlConnectionStringBuilder.ConnectionString;
                }

                var loggedSqlCommandFilterNodeName = sqlLogNode.FullName + ConfigurationNode.Delimiter + "LoggedSqlCommandFilter";
                var filter = new SimpleLoggedSqlCommandFilter(section, loggedSqlCommandFilterNodeName);

                if (isSafe)
                    this.Factory = new SafeLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);
                else
                    this.Factory = new SqlLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);

                var thread = this.Factory.Thread;
                thread.Start();
            }
            else
            {
                if (isSafe)
                    this.Factory = new SafeSqlConnectionFactory();
                else
                    this.Factory = new NativeSqlCommandFactory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout { get; }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnectionFactory Factory { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDbConnection CreateConnection(string name)
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
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(this._connectionString);
                sqlConnectionStringBuilder.ApplicationName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", sqlConnectionStringBuilder.ApplicationName,
                    name);
                connectionString = sqlConnectionStringBuilder.ConnectionString;
            }
            else
            {
                connectionString = this._connectionString;
            }

            return this.Factory.CreateConnection(connectionString, userName, hostName);
        }
    }
}