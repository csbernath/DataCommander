namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;

    internal sealed class InternalConnectionHelper : IInternalConnectionHelper
    {
        private static readonly FieldInfo internalConnectionField;

        private static FieldInfo isOpenField;

        static InternalConnectionHelper()
        {
            internalConnectionField = typeof(SqlConnection).GetField(
                "_internalConnection",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Type internalConnectionType = internalConnectionField.FieldType;
            isOpenField = internalConnectionType.GetField(
                "_fConnectionOpen",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        Object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
        {
            Object internalConnection = internalConnectionField.GetValue(connection);
            return internalConnection;
        }

        Boolean IInternalConnectionHelper.IsOpen(Object internalConnection)
        {
            Object value = isOpenField.GetValue(internalConnection);
            Boolean isOpen = (Boolean)value;
            return isOpen;
        }
    }
}