namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;

    internal interface IInternalConnectionHelper
    {
        Object GetInternalConnection(IDbConnection connection);

        Boolean IsOpen(Object internalConnection);
    }
}