namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;

    internal sealed class InternalConnectionHelper2 : IInternalConnectionHelper
    {
        private static readonly FieldInfo internalConnectionField;

        static InternalConnectionHelper2()
        {
            internalConnectionField = typeof(SqlConnection).GetField(
                "_innerConnection",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        Object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
        {
            Object internalConnection = internalConnectionField.GetValue(connection);
            return internalConnection;
        }

        Boolean IInternalConnectionHelper.IsOpen(Object internalConnection)
        {
            Type type = internalConnection.GetType();
            FieldInfo isOpenField = type.GetField("_fConnectionOpen", BindingFlags.Instance | BindingFlags.NonPublic);
            Object value = isOpenField.GetValue(internalConnection);
            Boolean isOpen = (Boolean)value;
            return isOpen;
        }
    }
}