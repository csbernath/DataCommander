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
        object internalConnection = InternalConnectionField.GetValue(connection);
        return internalConnection;
    }

    bool IInternalConnectionHelper.IsOpen(object internalConnection)
    {
        System.Type type = internalConnection.GetType();
        FieldInfo isOpenField = type.GetField("_fConnectionOpen", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = isOpenField.GetValue(internalConnection);
        bool isOpen = (bool)value;
        return isOpen;
    }
}