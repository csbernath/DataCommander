using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class StatisticsNode(DatabaseNode databaseNode, string? name) : ITreeNode
{
    private readonly DatabaseNode _databaseNode = databaseNode;

    public string? Name => name;

    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) => Task.FromResult(Enumerable.Empty<ITreeNode>());
    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    //private void menuItemScriptObject_Click(object? sender, EventArgs e)
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