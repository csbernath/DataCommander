using System;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using Foundation.Configuration;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public sealed class SimpleSqlConnectionFactory
{
    private readonly string _connectionString;

    public SimpleSqlConnectionFactory(ConfigurationSection section, string nodeName)
    {
        ArgumentNullException.ThrowIfNull(section);

        ConfigurationNode node = section.SelectNode(nodeName, true);
        _connectionString = node.Attributes["ConnectionString"].GetValue<string>();

        bool contains = node.Attributes.TryGetAttributeValue("CommandTimeout", out TimeSpan timeSpan);

        if (contains)
            CommandTimeout = (int)timeSpan.TotalSeconds;
        else
            CommandTimeout = 259200; // 3 days

        node.Attributes.TryGetAttributeValue("IsSafe", out bool isSafe);
        ConfigurationNode sqlLogNode = node.ChildNodes["SqlLog"];
        sqlLogNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);

        if (enabled)
        {
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            string applicationName = sqlConnectionStringBuilder.ApplicationName;
            contains = sqlLogNode.Attributes.TryGetAttributeValue("ConnectionString", null, out string logConnectionString);

            if (!contains)
            {
                contains = sqlLogNode.Attributes.TryGetAttributeValue("Data Source", null, out string dataSource);

                if (!contains)
                    dataSource = sqlConnectionStringBuilder.DataSource;

                string initialCatalog = sqlLogNode.Attributes["Initial Catalog"].GetValue<string>();

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

            string loggedSqlCommandFilterNodeName = sqlLogNode.FullName + ConfigurationNode.Delimiter + "LoggedSqlCommandFilter";
            SimpleLoggedSqlCommandFilter filter = new SimpleLoggedSqlCommandFilter(section, loggedSqlCommandFilterNodeName);

            if (isSafe)
                Factory = new SafeLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);
            else
                Factory = new SqlLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);

            Threading.WorkerThread thread = Factory.Thread;
            thread.Start();
        }
        else
        {
            if (isSafe)
                Factory = new SafeSqlConnectionFactory();
            else
                Factory = new NativeSqlCommandFactory();
        }
    }

    public int CommandTimeout { get; }

    public IDbConnectionFactory Factory { get; }

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
            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            sqlConnectionStringBuilder.ApplicationName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", sqlConnectionStringBuilder.ApplicationName,
                name);
            connectionString = sqlConnectionStringBuilder.ConnectionString;
        }
        else
        {
            connectionString = _connectionString;
        }

        return Factory.CreateConnection(connectionString, userName, hostName);
    }
}