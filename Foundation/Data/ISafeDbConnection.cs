using System;
using System.Data;
using System.Threading;

namespace Foundation.Data;

public interface ISafeDbConnection
{
    CancellationToken CancellationToken { get; }
    object Id { get; }
    void HandleException(Exception exception, TimeSpan elapsed);
    void HandleException(Exception exception, IDbCommand command);
}