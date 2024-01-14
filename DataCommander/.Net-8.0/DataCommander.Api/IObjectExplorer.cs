using System.Collections.Generic;
using System.Data;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface IObjectExplorer
{
    bool Sortable { get; }
    void SetConnection(string connectionString, SecureString? password, IDbConnection connection);
    Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken);
}