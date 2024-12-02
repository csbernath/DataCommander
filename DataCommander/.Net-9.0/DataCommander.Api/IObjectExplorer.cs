using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;

namespace DataCommander.Api;

public interface IObjectExplorer
{
    bool Sortable { get; }
    void SetConnectionStringAndCredential(ConnectionStringAndCredential connectionStringAndCredential);
    Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken);
}