namespace DataCommander.Foundation.Data.SqlClient.SqlLog
{
    using System.Data;

    internal interface IInternalConnectionHelper
    {
        object GetInternalConnection(IDbConnection connection);

        bool IsOpen(object internalConnection);
    }
}