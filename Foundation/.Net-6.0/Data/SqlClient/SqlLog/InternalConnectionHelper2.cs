using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class InternalConnectionHelper2 : IInternalConnectionHelper
{
    private static readonly FieldInfo InternalConnectionField;

    static InternalConnectionHelper2()
    {
        InternalConnectionField = typeof(SqlConnection).GetField(
            "_innerConnection",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
    {
        var internalConnection = InternalConnectionField.GetValue(connection);
        return internalConnection;
    }

    bool IInternalConnectionHelper.IsOpen(object internalConnection)
    {
        var type = internalConnection.GetType();
        var isOpenField = type.GetField("_fConnectionOpen", BindingFlags.Instance | BindingFlags.NonPublic);
        var value = isOpenField.GetValue(internalConnection);
        var isOpen = (bool)value;
        return isOpen;
    }
}