using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using DataCommander.Api.Connection;
using Foundation.Configuration;
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

        var provider = ProviderFactory.CreateProvider(providerIdentifier);
        var password = GetPassword(configurationNode);
        var connectionProperties = new ConnectionProperties(connectionName, providerIdentifier, provider, connectionString, password);
        return connectionProperties;
    }

    public static void Save(ConnectionProperties connectionProperties, ConfigurationNode configurationNode)
    {
        var attributes = configurationNode.Attributes;
        attributes.SetAttributeValue("ConnectionName", connectionProperties.ConnectionName);
        attributes.SetAttributeValue("ProviderIdentifier", connectionProperties.ProviderIdentifier);

        if (connectionProperties.Password != null)
        {
            var base64EncodedPassword = Convert.ToBase64String(connectionProperties.Password);
            attributes.SetAttributeValue(ConnectionStringKeyword.Password, base64EncodedPassword);
        }
        else
            attributes.Remove(ConnectionStringKeyword.Password);

        var connectionStringBuilder = new DbConnectionStringBuilder();
        connectionStringBuilder.ConnectionString = connectionProperties.ConnectionString;
        connectionStringBuilder.Remove(ConnectionStringKeyword.Password);
        attributes.SetAttributeValue("ConnectionString", connectionStringBuilder.ConnectionString);
    }

    private static byte[]? GetPassword(ConfigurationNode node)
    {
        byte[]? password = null;        
        var contains = node.Attributes.TryGetAttributeValue(ConnectionStringKeyword.Password, out string base64EncodedProtectedPassword);
        if (contains)
        {
            try
            {
                password = Convert.FromBase64String(base64EncodedProtectedPassword);
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }
        }

        return password;
    }

    public static byte[] ProtectPassword(string password)
    {
        var bytes = !string.IsNullOrEmpty(password)
            ? Encoding.UTF8.GetBytes(password)
            : Array.Empty<byte>();
        var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
        return protectedBytes;
    }

    public static string UnprotectPassword(byte[] protectedPassword)
    {
        var bytes = ProtectedData.Unprotect(protectedPassword, Entropy, DataProtectionScope.CurrentUser);
        var password = Encoding.UTF8.GetString(bytes);
        return password;
    }
}