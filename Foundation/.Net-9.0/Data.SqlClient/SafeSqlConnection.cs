using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using Foundation.Log;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

/// <summary>
/// Safe SQL Server connection for Windows Services.
/// </summary>
public class SafeSqlConnection : SafeDbConnection, ISafeDbConnection, ICloneable
{
    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(SafeSqlConnection));
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private short _id;

    public SafeSqlConnection(string connectionString)
    {
        var connection = new SqlConnection(connectionString);
        Initialize(connection, this);
    }

    public SafeSqlConnection(IDbConnection connection) => Initialize(connection, this);

    object ICloneable.Clone()
    {
        var connectionString = ConnectionString;
        var connection = new SafeSqlConnection(connectionString);
        return connection;
    }

    internal static short GetId(IDbConnection connection)
    {
        var executor = new DbCommandExecutor((DbConnection)connection);
        short id = 0;

        try
        {
            id = (short)executor.ExecuteScalar(new CreateCommandRequest("select @@spid"));
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, "Exception:\r\n{0}", e.ToLogString());
        }

        return id;
    }

    CancellationToken ISafeDbConnection.CancellationToken => _cancellationToken;

    object ISafeDbConnection.Id
    {
        get
        {
            if (_id == 0)
                _id = GetId(Connection);

            return _id;
        }
    }

    internal static void HandleException(IDbConnection connection, Exception exception, TimeSpan elapsed, CancellationToken cancellationToken)
    {
        var separator = new string('-', 80);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("SafeSqlConnection.HandleException(connection), elapsed: {0}, exception:\r\n{1}", elapsed, exception.ToLogString());
        var sqlException = exception as SqlException;
        var handled = false;
        var timeout = 1 * 60 * 1000; // 1 minutes

        if (sqlException != null)
        {
            switch (sqlException.Number)
            {
                case -2: // Timeout expired. The timeout period elapsed prior completion of the operation or the server is not responding.
                    handled = true;
                    timeout = 500; // 500 milliseconds
                    break;

                case 2:
                    // A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections
                    handled = true;
                    timeout = 5 * 1000; // 5 seconds
                    break;

                case SqlErrorNumber.CannotOpenDatabaseRequestedInLoginLoginFails:
                    handled = true;
                    timeout = 5 * 1000; // 5 seconds;
                    break;

                default:
                    if (connection.State != ConnectionState.Open)
                        handled = true;
                    break;
            }
        }
        else
        {
            if (exception is Win32Exception win32Exception)
            {
                // The wait operation timed out
                if (win32Exception.NativeErrorCode == 258)
                    timeout = 0;
            }
            else if (connection.State != ConnectionState.Open)
                handled = true;
        }

        if (handled)
        {
            stringBuilder.AppendFormat("\r\nWaiting {0}...", TimeSpan.FromMilliseconds(timeout));
            Log.Error(stringBuilder.ToString());

            if (timeout > 0)
                cancellationToken.WaitHandle.WaitOne(timeout);
        }
        else
            throw exception;
    }

    void ISafeDbConnection.HandleException(Exception exception, TimeSpan elapsed) => HandleException(Connection, exception, elapsed, _cancellationToken);

    internal static void HandleException(Exception exception, IDbCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var separator = new string('-', 80);
        var sb = new StringBuilder();
        sb.AppendLine("SafeSqlConnection.HandleException(command):\r\n");
        var parameters = (SqlParameterCollection)command.Parameters;
        var p = parameters.ToLogString();
        var database = command.Connection.Database;

        sb.AppendFormat("Database: {0}\r\n", database);
        sb.AppendFormat("Command: {0}\r\n{1}\r\n{2}\r\n", command.CommandText, p, separator);
        sb.AppendFormat("Exception:{0}\r\n{1}\r\n", exception, separator);
        var sqlEx = exception as SqlException;
        var handled = false;

        if (sqlEx != null)
        {
            sb.AppendFormat("{0}\r\n{1}\r\n", sqlEx.Errors.ToLogString(), separator);

            switch (sqlEx.Number)
            {
                case 11: // General network error.  Check your network documentation.
                    handled = true;
                    break;

                case SqlErrorNumber.DatabaseIsBeingRecovered:
                    handled = true;
                    break;

                case SqlErrorNumber.TransactionWasDeadlocked:
                    handled = true;
                    break;
            }
        }

        Log.Error(sb.ToString());

        if (handled)
            cancellationToken.WaitHandle.WaitOne(1 * 60 * 1000); // 1 minutes
        else
            throw exception;
    }

    void ISafeDbConnection.HandleException(Exception exception, IDbCommand command) => HandleException(exception, command, _cancellationToken);
}