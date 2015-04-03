namespace DataCommander.Foundation.Data.SqlClient
{
    using System.Data;

    internal interface IInternalConnectionHelper
    {
        object GetInternalConnection(IDbConnection connection);

        bool IsOpen(object internalConnection);
    }
}