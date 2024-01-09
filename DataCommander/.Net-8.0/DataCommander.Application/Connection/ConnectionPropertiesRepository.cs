using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using DataCommander.Api.Connection;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Log;

namespace DataCommander.Application.Connection;

public static class ConnectionPropertiesRepository
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private static readonly byte[] Entropy = [0x56, 0x4f, 0x3d, 0x78, 0xf1];

    public static ConnectionProperties GetFromConfiguration(ConfigurationNode configurationNode)
    {
        var attributes = configurationNode.Attributes;
        var connectionName = attributes["ConnectionName"].GetValue<string>();
        var providerIdentifier = attributes["ProviderIdentifier"].GetValue<string>();
        var connectionString = attributes["ConnectionString"].GetValue<string>();

        var connectionProperties = new ConnectionProperties(connectionName, providerIdentifier, null);
        connectionProperties.ConnectionString = connectionString;

        LoadProtectedPassword(configurationNode, connectionProperties);

        return connectionProperties;
    }

    public static void Save(ConnectionProperties connectionProperties, ConfigurationNode configurationNode)
    {
        var attributes = configurationNode.Attributes;
        attributes.SetAttributeValue("ConnectionName", connectionProperties.ConnectionName);
        attributes.SetAttributeValue("ProviderIdentifier", connectionProperties.ProviderIdentifier);

        if (connectionProperties.Password != null)
            attributes.SetAttributeValue(ConnectionStringKeyword.Password, ProtectPassword(connectionProperties.Password.Value));
        else
            attributes.Remove(ConnectionStringKeyword.Password);

        var connectionStringBuilder = new DbConnectionStringBuilder();
        connectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
        connectionStringBuilder.Remove(ConnectionStringKeyword.Password);
        attributes.SetAttributeValue("ConnectionString", connectionStringBuilder.ConnectionString);
    }

    private static string ProtectPassword(string password)
    {
        var bytes = !string.IsNullOrEmpty(password) ? Encoding.UTF8.GetBytes(password) : Array.Empty<byte>();
        var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
        var protectedPassword = Convert.ToBase64String(protectedBytes);
        return protectedPassword;
    }

    private static string UnprotectPassword(string protectedPassword)
    {
        var protectedBytes = Convert.FromBase64String(protectedPassword);
        var bytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
        var password = Encoding.UTF8.GetString(bytes);
        return password;
    }

    private static void LoadProtectedPassword(ConfigurationNode node, ConnectionProperties connectionProperties)
    {
        var contains = node.Attributes.TryGetAttributeValue(ConnectionStringKeyword.Password, out string password);
        if (contains)
        {
            var succeeded = false;
            try
            {
                password = UnprotectPassword(password);
                succeeded = true;
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }

            if (succeeded)
            {
                connectionProperties.Password = new Option<string>(password);

                var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                dbConnectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
                dbConnectionStringBuilder[ConnectionStringKeyword.Password] = password;
                connectionProperties.ConnectionString = dbConnectionStringBuilder.ConnectionString;
            }
        }
    }
}