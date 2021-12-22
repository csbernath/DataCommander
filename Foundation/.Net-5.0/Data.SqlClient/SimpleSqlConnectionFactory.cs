using System;
using System.Data;
using System.Globalization;
using System.Security.Principal;
using Foundation.Assertions;
using Foundation.Configuration;
using Foundation.Data.SqlClient.SqlLoggedSqlConnection;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;
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
        Assert.IsNotNull(section);

        var node = section.SelectNode(nodeName, true);
        _connectionString = node.Attributes["ConnectionString"].GetValue<string>();

        var contains = node.Attributes.TryGetAttributeValue("CommandTimeout", out TimeSpan timeSpan);

        if (contains)
            CommandTimeout = (int)timeSpan.TotalSeconds;
        else
            CommandTimeout = 259200; // 3 days

        node.Attributes.TryGetAttributeValue("IsSafe", out bool isSafe);
        var sqlLogNode = node.ChildNodes["SqlLog"];
        sqlLogNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);

        if (enabled)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            var applicationName = sqlConnectionStringBuilder.ApplicationName;
            contains = sqlLogNode.Attributes.TryGetAttributeValue("ConnectionString", null, out string logConnectionString);

            if (!contains)
            {
                contains = sqlLogNode.Attributes.TryGetAttributeValue("Data Source", null, out string dataSource);

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
                Factory = new SafeLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);
            else
                Factory = new SqlLoggedSqlConnectionFactory(logConnectionString, applicationName, filter);

            var thread = Factory.Thread;
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
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
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