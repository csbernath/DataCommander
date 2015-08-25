namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using Foundation.Data;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    internal sealed class PackageCollectionNode : ITreeNode
    {
        public PackageCollectionNode(SchemaNode schema)
        {
            this.schema = schema;
        }

        public string Name
        {
            get
            {
                return "Packages";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ConfigurationNode folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
            string key = schema.SchemasNode.Connection.DataSource + "." + schema.Name;
            string[] packages;
            bool contains = folder.Attributes.TryGetAttributeValue(key, out packages);

            if (!contains || refresh)
            {
                string commandText = "select object_name from all_objects where owner = '{0}' and object_type = 'PACKAGE' order by object_name";
                commandText = string.Format(commandText, schema.Name);
                var transactionScope = new DbTransactionScope(this.schema.SchemasNode.Connection, null);
                DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
                int count = dataTable.Rows.Count;
                packages = new string[count];

                for (int i = 0; i < count; i++)
                {
                    packages[i] = (string)dataTable.Rows[i][0];
                }

                folder.Attributes.SetAttributeValue(key, packages);
            }

            var treeNodes = new ITreeNode[packages.Length];

            for (int i = 0; i < packages.Length; i++)
                treeNodes[i] = new PackageNode(schema, packages[i]);

            return treeNodes;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                return null;
            }
        }

        public SchemaNode Schema
        {
            get
            {
                return schema;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        public void BeforeExpand()
        {
        }

        readonly SchemaNode schema;
    }
}