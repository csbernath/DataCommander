using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Foundation.Data.SqlClient.SqlLog
{
    internal sealed class InternalConnectionHelper : IInternalConnectionHelper
    {
        private static readonly FieldInfo InternalConnectionField;

        private static readonly FieldInfo IsOpenField;

        static InternalConnectionHelper()
        {
            InternalConnectionField = typeof(SqlConnection).GetField(
                "_internalConnection",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var internalConnectionType = InternalConnectionField.FieldType;
            IsOpenField = internalConnectionType.GetField(
                "_fConnectionOpen",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
        {
            var internalConnection = InternalConnectionField.GetValue(connection);
            return internalConnection;
        }

        bool IInternalConnectionHelper.IsOpen(object internalConnection)
        {
            var value = IsOpenField.GetValue(internalConnection);
            var isOpen = (bool)value;
            return isOpen;
        }
    }
}