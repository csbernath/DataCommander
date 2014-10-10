namespace DataCommander.Providers
{
    using System;
    using System.Data.Common;
    using System.Security.Cryptography;
    using System.Text;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Diagnostics;

    public sealed class ConnectionProperties
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        public string connectionName;
        public string providerName;
        public IProvider provider;
        public string connectionString;
        public ConnectionBase connection;
        private static byte[] entropy = new byte[] { 0x56, 0x4f, 0x3d, 0x78, 0xf1 };

        public static string GetValue(DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
        {
            object obj;
            bool contains = dbConnectionStringBuilder.TryGetValue(keyword, out obj);
            string value;

            if (contains)
            {
                value = (string)obj;
            }
            else
            {
                value = null;
            }

            return value;
        }

        public static string ProtectPassword(string password)
        {
            byte[] bytes;

            if (!string.IsNullOrEmpty(password))
            {
                bytes = Encoding.UTF8.GetBytes(password);
            }
            else
            {
                bytes = new byte[0];
            }

            byte[] protectedBytes = ProtectedData.Protect(bytes, entropy, DataProtectionScope.CurrentUser);
            string protectedPassword = Convert.ToBase64String(protectedBytes);
            return protectedPassword;
        }

        public static string UnprotectPassword(string protectedPassword)
        {
            byte[] protectedBytes = Convert.FromBase64String(protectedPassword);
            byte[] bytes = ProtectedData.Unprotect(protectedBytes, entropy, DataProtectionScope.CurrentUser);
            string password = Encoding.UTF8.GetString(bytes);
            return password;
        }

        public void Save(ConfigurationNode folder)
        {
            ConfigurationAttributeCollection attributes = folder.Attributes;
            attributes.SetAttributeValue("ConnectionName", connectionName);
            attributes.SetAttributeValue("ProviderName", providerName);
            attributes.SetAttributeValue("ConnectionString", connectionString);
        }

        public void Load(ConfigurationNode folder)
        {
            ConfigurationAttributeCollection attributes = folder.Attributes;
            connectionName = attributes["ConnectionName"].GetValue<string>();
            providerName = attributes["ProviderName"].GetValue<string>();
            connectionString = attributes["ConnectionString"].GetValue<string>();
        }

        public void LoadProtectedPassword(ConfigurationNode node)
        {
            string password;
            bool contains = node.Attributes.TryGetAttributeValue<string>("Password", out password);
            if (contains)
            {
                bool succeeded = false;
                try
                {
                    password = ConnectionProperties.UnprotectPassword(password);
                    succeeded = true;
                }
                catch (Exception e)
                {
                    log.Write(LogLevel.Error, e.ToString());
                }

                if (succeeded)
                {
                    DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
                    dbConnectionStringBuilder.ConnectionString = this.connectionString;
                    dbConnectionStringBuilder["Password"] = password;
                    this.connectionString = dbConnectionStringBuilder.ConnectionString;
                }
            }
        }
    }
}