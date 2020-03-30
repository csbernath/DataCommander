using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using DataCommander.Providers2.Connection;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Log;

namespace DataCommander.Providers.Connection
{
    public static class ConnectionPropertiesRepository
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly byte[] Entropy = {0x56, 0x4f, 0x3d, 0x78, 0xf1};

        public static ConnectionProperties GetFromConfiguration(ConfigurationNode configurationNode)
        {
            var connectionProperties = new ConnectionProperties();

            var attributes = configurationNode.Attributes;
            connectionProperties.ConnectionName = attributes["ConnectionName"].GetValue<string>();
            connectionProperties.ProviderName = attributes["ProviderName"].GetValue<string>();
            connectionProperties.ConnectionString = attributes["ConnectionString"].GetValue<string>();

            if (attributes.TryGetValue(ConnectionStringKeyword.DataSource, out var attribute))
                connectionProperties.DataSource = attribute.GetValue<string>();

            if (attributes.TryGetValue(ConnectionStringKeyword.InitialCatalog, out attribute))
                connectionProperties.InitialCatalog = attribute.GetValue<string>();

            if (attributes.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out attribute))
                connectionProperties.IntegratedSecurity = attribute.Value != null
                    ? (bool?) attribute.GetValue<bool>()
                    : null;

            if (attributes.TryGetValue(ConnectionStringKeyword.UserId, out attribute))
                connectionProperties.UserId = attribute.GetValue<string>();

            LoadProtectedPassword(configurationNode, connectionProperties);

            return connectionProperties;
        }

        public static void Save(ConnectionProperties connectionProperties, ConfigurationNode configurationNode)
        {
            var attributes = configurationNode.Attributes;
            attributes.SetAttributeValue("ConnectionName", connectionProperties.ConnectionName);
            attributes.SetAttributeValue("ProviderName", connectionProperties.ProviderName);
            attributes.SetAttributeValue(ConnectionStringKeyword.DataSource, connectionProperties.DataSource);
            attributes.SetAttributeValue(ConnectionStringKeyword.InitialCatalog, connectionProperties.InitialCatalog);
            attributes.SetAttributeValue(ConnectionStringKeyword.IntegratedSecurity, connectionProperties.IntegratedSecurity);
            attributes.SetAttributeValue(ConnectionStringKeyword.UserId, connectionProperties.UserId);

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
            var bytes = !string.IsNullOrEmpty(password) ? Encoding.UTF8.GetBytes(password) : new byte[0];
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
}