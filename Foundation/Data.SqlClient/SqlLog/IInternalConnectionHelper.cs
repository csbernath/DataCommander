using System.Data;

namespace Foundation.Data.SqlClient.SqlLog;

internal interface IInternalConnectionHelper
{
    object GetInternalConnection(IDbConnection connection);

    bool IsOpen(object internalConnection);
}