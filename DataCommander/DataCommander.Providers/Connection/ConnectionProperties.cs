using System;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using Foundation.Configuration;
using Foundation.Log;

namespace DataCommander.Providers.Connection
{
    public sealed class ConnectionProperties
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        public string ConnectionName;
        public string ProviderName;

        public string DataSource;
        public string InitialCatalog;
        public bool? IntegratedSecurity;
        public string UserId;

        public IProvider Provider;
        public string ConnectionString;
        public ConnectionBase Connection;
        private static readonly byte[] Entropy = {0x56, 0x4f, 0x3d, 0x78, 0xf1};

        public static string ProtectPassword(string password)
        {
            var bytes = !string.IsNullOrEmpty(password) ? Encoding.UTF8.GetBytes(password) : new byte[0];
            var protectedBytes = ProtectedData.Protect(bytes, Entropy, DataProtectionScope.CurrentUser);
            var protectedPassword = Convert.ToBase64String(protectedBytes);
            return protectedPassword;
        }

        public static string UnprotectPassword(string protectedPassword)
        {
            var protectedBytes = Convert.FromBase64String(protectedPassword);
            var bytes = ProtectedData.Unprotect(protectedBytes, Entropy, DataProtectionScope.CurrentUser);
            var password = Encoding.UTF8.GetString(bytes);
            return password;
        }

        public void Save(ConfigurationNode folder)
        {
            var attributes = folder.Attributes;
            attributes.SetAttributeValue("ConnectionName", ConnectionName);
            attributes.SetAttributeValue("ProviderName", ProviderName);
            attributes.SetAttributeValue(ConnectionStringKeyword.DataSource, DataSource);
            attributes.SetAttributeValue(ConnectionStringKeyword.InitialCatalog, InitialCatalog);
            attributes.SetAttributeValue(ConnectionStringKeyword.IntegratedSecurity, IntegratedSecurity);
            attributes.SetAttributeValue(ConnectionStringKeyword.UserId, UserId);
            attributes.SetAttributeValue("ConnectionString", ConnectionString);
        }

        public void Load(ConfigurationNode folder)
        {
            var attributes = folder.Attributes;
            ConnectionName = attributes["ConnectionName"].GetValue<string>();
            ProviderName = attributes["ProviderName"].GetValue<string>();
            ConnectionString = attributes["ConnectionString"].GetValue<string>();

            ConfigurationAttribute attribute;
            if (attributes.TryGetValue(ConnectionStringKeyword.DataSource, out attribute))
                DataSource = attribute.GetValue<string>();

            if (attributes.TryGetValue(ConnectionStringKeyword.InitialCatalog, out attribute))
                InitialCatalog = attribute.GetValue<string>();

            if (attributes.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out attribute))
            {
                if (attribute.Value != null)
                    IntegratedSecurity = attribute.GetValue<bool>();
                else
                    IntegratedSecurity = null;
            }

            if (attributes.TryGetValue(ConnectionStringKeyword.UserId, out attribute))
                UserId = attribute.GetValue<string>();
        }

        public void LoadProtectedPassword(ConfigurationNode node)
        {
            string password;
            var contains = node.Attributes.TryGetAttributeValue("Password", out password);
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
                    var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                    dbConnectionStringBuilder.ConnectionString = ConnectionString;
                    dbConnectionStringBuilder[ConnectionStringKeyword.Password] = password;
                    ConnectionString = dbConnectionStringBuilder.ConnectionString;
                }
            }
        }
    }
}