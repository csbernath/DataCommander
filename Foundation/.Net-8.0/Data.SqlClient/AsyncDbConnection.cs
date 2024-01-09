using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using Foundation.Log;
using Foundation.Threading;
using Microsoft.Data.SqlClient;

namespace Foundation.Data.SqlClient;

public sealed class AsyncDbConnection : IDbConnection
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(AsyncDbConnection));
    private readonly IDbConnection _cloneableConnection;
    private readonly ICloneable _cloneable;
    private readonly List<string> _commands = [];
    private readonly AutoResetEvent _queueEvent = new(false);
    private readonly WorkerThread _thread;

    #endregion

    public AsyncDbConnection(IDbConnection cloneableConnection, string threadName)
    {
        _cloneableConnection = cloneableConnection;
        _cloneable = (ICloneable)cloneableConnection;
        _thread = new WorkerThread(Start);
        _thread.Name = threadName;
        _thread.Start();
    }

    #region IDbConnection Members

    public void ChangeDatabase(string databaseName)
    {
        // TODO:  Add AsyncSqlConnection.ChangeDatabase implementation
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        // TODO:  Add AsyncSqlConnection.BeginTransaction implementation
        return null;
    }

    IDbTransaction IDbConnection.BeginTransaction()
    {
        // TODO:  Add AsyncSqlConnection.System.Data.IDbConnection.BeginTransaction implementation
        return null;
    }

    public ConnectionState State => _cloneableConnection.State;

    public string ConnectionString
    {
        get => _cloneableConnection.ConnectionString;
        set => _cloneableConnection.ConnectionString = value;
    }

    public IDbCommand CreateCommand()
    {
        var command = _cloneableConnection.CreateCommand();
        return new AsyncDbCommand(this, command);
    }

    public void Open()
    {
        // TODO:  Add AsyncSqlConnection.Open implementation
    }

    public void Close()
    {
        _thread.Stop();
        _thread.Join();
    }

    public string Database => _cloneableConnection.Database;
    public int ConnectionTimeout => _cloneableConnection.ConnectionTimeout;

    #endregion

    #region IDisposable Members

    public void Dispose() => Close();

    #endregion

    #region Internal Methods

    internal int ExecuteNonQuery(AsyncDbCommand command)
    {
        var commandText = ToString(command);

        lock (_commands)
            _commands.Add(commandText);

        _queueEvent.Set();

        return 0;
    }

    #endregion

    #region Private Methods

    private static string ToString(IDbCommand command)
    {
        string commandText;

        switch (command.CommandType)
        {
            case CommandType.StoredProcedure:
                var sb = new StringBuilder();
                sb.AppendFormat("exec {0}", command.CommandText);
                var parameters = (SqlParameterCollection)command.Parameters;
                var parametersString = IDataParameterCollectionExtensions.ToLogString(parameters);

                if (parametersString.Length > 0)
                {
                    sb.Append(' ');
                    sb.Append(parametersString);
                }

                commandText = sb.ToString();
                break;

            case CommandType.Text:
                commandText = command.CommandText;
                break;

            default:
                throw new NotImplementedException();
        }

        return commandText;
    }

    private void Start()
    {
        WaitHandle[] waitHandles =
        [
            _thread.StopRequest,
            _queueEvent
        ];

        const int timeout = 10000; // 10 sec

        while (true)
        {
            Flush();

            if (_thread.IsStopRequested)
                break;

            WaitHandle.WaitAny(waitHandles, timeout, false);
        }
    }

    private void Flush()
    {
        if (_commands.Count > 0)
        {
            while (_commands.Count > 0)
            {
                string[] commandTextArray;

                lock (_commands)
                {
                    commandTextArray = new string[_commands.Count];
                    _commands.CopyTo(commandTextArray);
                    _commands.Clear();
                }

                var sb = new StringBuilder();

                for (var i = 0; i < commandTextArray.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(Environment.NewLine);
                    }

                    sb.Append(commandTextArray[i]);
                }

                var commandText = sb.ToString();
                Exception exception = null;

                using (var connection = (IDbConnection)_cloneable.Clone())
                {
                    var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandTimeout = 600;
                    connection.Open();

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }

                if (exception != null)
                {
                    var message = exception.ToLogString();
                    Log.Write(LogLevel.Error, message);
                }
            }
        }
    }

    #endregion
}