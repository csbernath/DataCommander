using System.Collections.Generic;
using System.Data;

namespace DataCommander.Providers2;

public interface IObjectExplorer
{
    bool Sortable { get; }
    void SetConnection(string connectionString, IDbConnection connection);
    IEnumerable<ITreeNode> GetChildren(bool refresh);
}