using System.Collections.Generic;
using System.Data;

namespace DataCommander.Api;

public interface IObjectExplorer
{
    bool Sortable { get; }
    void SetConnection(string connectionString, IDbConnection connection);
    IEnumerable<ITreeNode> GetChildren(bool refresh);
}