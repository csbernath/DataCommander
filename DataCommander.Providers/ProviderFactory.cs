namespace DataCommander.Providers
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;

    public static class ProviderFactory
    {
        public static List<string> Providers
        {
            get
            {
                List<string> providers = new List<string>();
                ConfigurationNode node = Settings.CurrentNamespace;

                foreach (var childNode in node.ChildNodes)
                {
                    providers.Add(childNode.Name);
                }

                return providers;
            }
        }

        public static IProvider CreateProvider(string name)
        {
            Contract.Requires( name != null );
            Contract.Ensures(Contract.Result<IProvider>() != null);
            ConfigurationNode folder = Settings.CurrentNamespace;
            folder = folder.ChildNodes[name];
            var attributes = folder.Attributes;
            string assemblyNameString = attributes["AssemblyName"].GetValue<string>();
            var assemblyName = new AssemblyName();
            assemblyName.Name = assemblyNameString;
            var assembly = Assembly.Load(assemblyName);
            string typeName = attributes["TypeName"].GetValue<string>();
            object instance = assembly.CreateInstance(typeName);
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
                var c = new ADODB.ConnectionClass();

                c.Open(connectionString, null, null, 0);
                ADODB.Recordset rs = c.OpenSchema(ADODB.SchemaEnum.adSchemaDBInfoKeywords, System.Type.Missing, System.Type.Missing);
                System.Data.DataTable dataTable = OleDBHelper.Convert(rs);
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