namespace DataCommander.Providers.Connection
{
    using System;
    using System.Data.Common;
    using System.Security.Cryptography;
    using System.Text;
    using Foundation.Configuration;
    using Foundation.Diagnostics;

    public sealed class ConnectionProperties
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        public string ConnectionName;
        public string ProviderName;

        public string DataSource;
        public string InitialCatalog;
        public bool? IntegratedSecurity;
        public string UserId;

        public IProvider Provider;
        public string ConnectionString;
        public ConnectionBase Connection;
        private static readonly byte[] entropy = {0x56, 0x4f, 0x3d, 0x78, 0xf1};

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

            var protectedBytes = ProtectedData.Protect(bytes, entropy, DataProtectionScope.CurrentUser);
            var protectedPassword = Convert.ToBase64String(protectedBytes);
            return protectedPassword;
        }

        public static string UnprotectPassword(string protectedPassword)
        {
            var protectedBytes = Convert.FromBase64String(protectedPassword);
            var bytes = ProtectedData.Unprotect(protectedBytes, entropy, DataProtectionScope.CurrentUser);
            var password = Encoding.UTF8.GetString(bytes);
            return password;
        }

        public void Save(ConfigurationNode folder)
        {
            var attributes = folder.Attributes;
            attributes.SetAttributeValue("ConnectionName", this.ConnectionName);
            attributes.SetAttributeValue("ProviderName", this.ProviderName);
            attributes.SetAttributeValue(ConnectionStringKeyword.DataSource, this.DataSource);
            attributes.SetAttributeValue(ConnectionStringKeyword.InitialCatalog,this.InitialCatalog);
            attributes.SetAttributeValue(ConnectionStringKeyword.IntegratedSecurity,this.IntegratedSecurity);
            attributes.SetAttributeValue(ConnectionStringKeyword.UserId,this.UserId);
            attributes.SetAttributeValue("ConnectionString", this.ConnectionString);
        }

        public void Load(ConfigurationNode folder)
        {
            var attributes = folder.Attributes;
            this.ConnectionName = attributes["ConnectionName"].GetValue<string>();
            this.ProviderName = attributes["ProviderName"].GetValue<string>();
            this.ConnectionString = attributes["ConnectionString"].GetValue<string>();

            ConfigurationAttribute attribute;
            if (attributes.TryGetValue(ConnectionStringKeyword.DataSource, out attribute))
            {
                this.DataSource = attribute.GetValue<string>();
            }

            if (attributes.TryGetValue(ConnectionStringKeyword.InitialCatalog, out attribute))
            {
                this.InitialCatalog = attribute.GetValue<string>();
            }

            if (attributes.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out attribute))
            {
                if (attribute.Value != null)
                {
                    this.IntegratedSecurity = attribute.GetValue<bool>();
                }
                else
                {
                    this.IntegratedSecurity = null;
                }
            }

            if (attributes.TryGetValue(ConnectionStringKeyword.UserId, out attribute))
            {
                this.UserId = attribute.GetValue<string>();
            }
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
                    log.Write(LogLevel.Error, e.ToString());
                }

                if (succeeded)
                {
                    var dbConnectionStringBuilder = new DbConnectionStringBuilder();
                    dbConnectionStringBuilder.ConnectionString = this.ConnectionString;
                    dbConnectionStringBuilder[ConnectionStringKeyword.Password] = password;
                    this.ConnectionString = dbConnectionStringBuilder.ConnectionString;
                }
            }
        }
    }
}