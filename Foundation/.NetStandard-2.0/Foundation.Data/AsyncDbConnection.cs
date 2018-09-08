using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Foundation.Data.SqlClient;
using Foundation.Log;
using Foundation.Threading;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncDbConnection : IDbConnection
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof(AsyncDbConnection));
        private readonly IDbConnection _cloneableConnection;
        private readonly ICloneable _cloneable;
        private readonly List<string> _commands = new List<string>();
        private readonly AutoResetEvent _queueEvent = new AutoResetEvent(false);
        private readonly WorkerThread _thread;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloneableConnection"></param>
        /// <param name="threadName"></param>
        public AsyncDbConnection(IDbConnection cloneableConnection, string threadName)
        {
            _cloneableConnection = cloneableConnection;
            _cloneable = (ICloneable) cloneableConnection;
            _thread = new WorkerThread(Start);
            _thread.Name = threadName;
            _thread.Start();
        }

        #region IDbConnection Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase(string databaseName)
        {
            // TODO:  Add AsyncSqlConnection.ChangeDatabase implementation
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => _cloneableConnection.State;

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get => _cloneableConnection.ConnectionString;

            set => _cloneableConnection.ConnectionString = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            var command = _cloneableConnection.CreateCommand();
            return new AsyncDbCommand(this, command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            // TODO:  Add AsyncSqlConnection.Open implementation
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _thread.Stop();
            _thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Database => _cloneableConnection.Database;

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => _cloneableConnection.ConnectionTimeout;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

        #region Internal Methods

        internal int ExecuteNonQuery(AsyncDbCommand command)
        {
            var commandText = ToString(command);

            lock (_commands)
            {
                _commands.Add(commandText);
            }

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
                    var parameters = (SqlParameterCollection) command.Parameters;
                    var parametersString = parameters.ToLogString();

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
            {
                _thread.StopRequest,
                _queueEvent
            };

            const int timeout = 10000; // 10 sec

            while (true)
            {
                Flush();

                if (_thread.IsStopRequested)
                {
                    break;
                }

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
                        string message;
                        var sqlException = exception as SqlException;

                        if (sqlException != null)
                        {
                            message = sqlException.Errors.ToLogString();
                        }
                        else
                        {
                            message = exception.ToLogString();
                        }

                        Log.Write(LogLevel.Error, message);
                    }
                }
            }
        }

        #endregion
    }
}