using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient.AppLock;

public sealed class AppLockMethods
{
    public static GetAppLockReturnCode GetAppLock(IDbConnection connection, IDbTransaction transaction, string resourceName, LockMode lockMode,
        LockOwner? lockOwner,
        TimeSpan? lockTimeout, string databasePrincipal)
    {
        const string commandText = "sp_getapplock";

        var builder = new SqlParameterCollectionBuilder();
        builder.Add("Resource", resourceName);
        builder.Add("LockMode", lockMode.ToString());

        if (lockOwner != null)
            builder.Add("LockOwner", lockOwner.ToString()!);

        if (lockTimeout != null)
            builder.Add("LockTimeout", (int)lockTimeout.Value.TotalMilliseconds);

        if (databasePrincipal != null)
            builder.Add("DbPrincipal", databasePrincipal);

        var returnCodeParameter = new SqlParameter
        {
            //ParameterName = "returnCode",
            //SqlDbType = SqlDbType.Int,
            Direction = ParameterDirection.ReturnValue
        };
        builder.Add(returnCodeParameter);
        var parameters = builder.ToReadOnlyCollection();

        var createCommandRequest = new CreateCommandRequest(commandText, parameters, CommandType.StoredProcedure, null, transaction);
        connection.CreateCommandExecutor().ExecuteNonQuery(createCommandRequest);

        var returnCode = (GetAppLockReturnCode)(int)returnCodeParameter.Value;
        return returnCode;
    }
}