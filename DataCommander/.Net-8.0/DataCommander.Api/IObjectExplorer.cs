using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface IObjectExplorer
{
    bool Sortable { get; }
    void SetConnection(string connectionString, IDbConnection connection);
    Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken);
}