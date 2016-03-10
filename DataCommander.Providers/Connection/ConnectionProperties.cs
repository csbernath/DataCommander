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
            ConfigurationAttributeCollection attributes = folder.Attributes;
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
            bool contains = node.Attributes.TryGetAttributeValue("Password", out password);
            if (contains)
            {
                bool succeeded = false;
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

    internal static class DbConnectionStringBuilderExtensions
    {
        public static string GetValue(this DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
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
    }
}