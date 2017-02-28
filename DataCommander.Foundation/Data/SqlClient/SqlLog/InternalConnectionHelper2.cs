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

        object IInternalConnectionHelper.GetInternalConnection(IDbConnection connection)
        {
            var internalConnection = internalConnectionField.GetValue(connection);
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
}