using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class IndexNode(DatabaseNode databaseNode, int parentId, int id, string name, byte type, bool isUnique)
    : ITreeNode
{
    private readonly DatabaseNode _databaseNode = databaseNode;
    private readonly int _id = id;
    private readonly int _parentId = parentId;

    public string Name
    {
        get
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(name);
            stringBuilder.Append(" (");

            stringBuilder.Append(isUnique
                ? "Unique"
                : "Non-Unique");

            stringBuilder.Append(',');

            string typeString;
            switch (type)
            {
                case 0:
                    typeString = "Heap";
                    break;

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

            stringBuilder.Append(typeString);
            stringBuilder.Append(')');

            return stringBuilder.ToString();
        }
    }

    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
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

    public ContextMenu? GetContextMenu() => null;
}