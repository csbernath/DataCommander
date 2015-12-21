namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using ADODB;
    using DataCommander.Foundation.Configuration;

    public static class ProviderFactory
    {
        public static List<string> Providers
        {
            get
            {
                var providers = new List<string>();
                ConfigurationNode node = Settings.CurrentNamespace;

                foreach (var childNode in node.ChildNodes)
                {
                    bool enabled;
                    childNode.Attributes.TryGetAttributeValue("Enabled", out enabled);

                    if (enabled)
                    {
                        providers.Add(childNode.Name);
                    }
                }

                return providers;
            }
        }

        public static IProvider CreateProvider(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Ensures(Contract.Result<IProvider>() != null);

            ConfigurationNode folder = Settings.CurrentNamespace;
            folder = folder.ChildNodes[name];
            var attributes = folder.Attributes;
            string typeName = attributes["TypeName"].GetValue<string>();
            Type type = Type.GetType(typeName, true);
            object instance = Activator.CreateInstance(type);
            Contract.Assert(instance != null);
            Contract.Assert(instance is IProvider);
            var provider = (IProvider)instance;

            return provider;
        }

        public static string[] GetKeyWords(string connectionString)
        {
            string[] keyWords = null;

            try
            {
                var c = new ConnectionClass();

                c.Open(connectionString, null, null, 0);
                Recordset rs = c.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                DataTable dataTable = OleDbHelper.Convert(rs);
                rs.Close();
                c.Close();

                keyWords = new string[dataTable.Rows.Count];

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    keyWords[i] = (string)dataTable.Rows[i][0];
                }
            }
            catch
            {
            }

            return keyWords;
        }
    }
}