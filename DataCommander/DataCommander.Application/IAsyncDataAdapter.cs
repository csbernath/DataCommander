using System.Collections.Generic;
using System.Threading;

namespace DataCommander.Application;

internal interface IAsyncDataAdapter
{
    IResultWriter ResultWriter { get; }
    long RowCount { get; }
    int TableCount { get; }

    void Start(IEnumerable<AsyncDataAdapterCommand>? commands, CancellationToken cancellationToken);
    void Cancel();
}