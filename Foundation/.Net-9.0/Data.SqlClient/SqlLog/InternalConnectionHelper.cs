using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.SqlLog;

internal sealed class InternalConnectionHelper : IInternalConnectionHelper
{
    private static readonly FieldInfo InternalConnectionField;

    private static readonly FieldInfo IsOpenField;

    static InternalConnectionHelper()
    {
        InternalConnectionField = typeof(SqlConnection).GetField(
            "_internalConnection",
            BindingFlags.Instance | BindingFlags.NonPublic);
        System.Type internalConnectionType = InternalConnectionField.FieldType;
        IsOpenField = internalConnectionType.GetField(
            "_fConnectionOpen",
            BindingFlags.Instance | BindingFlags.NonPublic);
    }

    object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
    {
        object internalConnection = InternalConnectionField.GetValue(connection);
        return internalConnection;
    }

    bool IInternalConnectionHelper.IsOpen(object internalConnection)
    {
        object value = IsOpenField.GetValue(internalConnection);
        bool isOpen = (bool)value;
        return isOpen;
    }
}