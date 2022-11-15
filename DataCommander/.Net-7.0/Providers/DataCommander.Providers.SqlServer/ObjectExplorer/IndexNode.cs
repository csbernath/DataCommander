using System.Collections.Generic;
using System.Text;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class IndexNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;
    private readonly int _id;
    private readonly bool _isUnique;
    private readonly string _name;
    private readonly int _parentId;
    private readonly byte _type;

    public IndexNode(DatabaseNode databaseNode, int parentId, int id, string name, byte type, bool isUnique)
    {
        _databaseNode = databaseNode;
        _parentId = parentId;
        _id = id;
        _name = name;
        _type = type;
        _isUnique = isUnique;
    }

    public string Name
    {
        get
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(_name);
            stringBuilder.Append(" (");

            stringBuilder.Append(_isUnique
                ? "Unique"
                : "Non-Unique");

            stringBuilder.Append(',');

            string typeString;
            switch (_type)
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

    public ContextMenu? GetContextMenu() => null;
}