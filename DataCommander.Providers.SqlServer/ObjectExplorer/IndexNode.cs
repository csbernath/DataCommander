namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    internal sealed class IndexNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly int _parentId;
        private readonly int _id;
        private readonly string name;
        private readonly byte type;
        private readonly bool isUnique;

        public IndexNode(DatabaseNode databaseNode, int parentId, int id, string name, byte type, bool isUnique)
        {
            _databaseNode = databaseNode;
            _parentId = parentId;
            _id = id;
            this.name = name;
            this.type = type;
            this.isUnique = isUnique;
        }

        public string Name
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(this.name);
                sb.Append('(');

                sb.Append(this.isUnique
                    ? "Unique"
                    : "Non-Unique");

                sb.Append(',');

                string typeString;
                switch (this.type)
                {
                    case 1:
                        typeString = "Clustered";
                        break;

                    case 2:
                        typeString = "Non-Clustered";
                        break;

                    default:
                        typeString = "???";
                        break;
                }

                sb.Append(typeString);
                sb.Append(')');

                return sb.ToString();
            }
        }

        public bool IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => null;

        //private void menuItemScriptObject_Click(object sender, EventArgs e)
        //{
        //    string connectionString = this.tableNode.
        //        .database.Databases.Server.ConnectionString;
        //    string text;
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        text = SqlDatabase.GetSysComments(connection, this.database.Name, "dbo", this.name);
        //    }
        //    QueryForm.ShowText(text);
        //}

        public ContextMenuStrip ContextMenu => null;
    }
}