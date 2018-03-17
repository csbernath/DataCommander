using System;
using System.Collections.Generic;
using ADODB;
using Foundation.Configuration;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers
{
    public static class ProviderFactory
    {
        public static List<string> Providers
        {
            get
            {
                var providers = new List<string>();
                var node = Settings.CurrentNamespace;

                foreach (var childNode in node.ChildNodes)
                {
                    childNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);

                    if (enabled)
                        providers.Add(childNode.Name);
                }

                return providers;
            }
        }

        public static IProvider CreateProvider(string name)
        {
            Assert.IsNotNull(name);
            //Contract.Ensures(Contract.Result<IProvider>() != null);

            var folder = Settings.CurrentNamespace;
            folder = folder.ChildNodes[name];
            var attributes = folder.Attributes;
            var typeName = attributes["TypeName"].GetValue<string>();
            var type = Type.GetType(typeName, true);
            var instance = Activator.CreateInstance(type);
            FoundationContract.Assert(instance != null);
            FoundationContract.Assert(instance is IProvider);
            var provider = (IProvider) instance;

            return provider;
        }

        public static string[] GetKeyWords(string connectionString)
        {
            string[] keyWords = null;

            try
            {
                var c = new ConnectionClass();

                c.Open(connectionString, null, null, 0);
                var rs = c.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                var dataTable = OleDbHelper.Convert(rs);
                rs.Close();
                c.Close();

                keyWords = new string[dataTable.Rows.Count];

                for (var i = 0; i < dataTable.Rows.Count; i++)
                    keyWords[i] = (string) dataTable.Rows[i][0];
            }
            catch
            {
            }

            return keyWords;
        }
    }
}