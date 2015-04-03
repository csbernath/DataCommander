namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AsyncDbConnection : IDbConnection
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IDbConnection cloneableConnection;
        private readonly ICloneable cloneable;
        private readonly List<string> commands = new List<string>();
        private readonly AutoResetEvent queueEvent = new AutoResetEvent( false );
        private readonly WorkerThread thread;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloneableConnection"></param>
        /// <param name="threadName"></param>
        public AsyncDbConnection( IDbConnection cloneableConnection, string threadName )
        {
            this.cloneableConnection = cloneableConnection;
            this.cloneable = (ICloneable) cloneableConnection;
            this.thread = new WorkerThread( this.Start );
            this.thread.Name = threadName;
            this.thread.Start();
        }

        #region IDbConnection Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseName"></param>
        public void ChangeDatabase( string databaseName )
        {
            // TODO:  Add AsyncSqlConnection.ChangeDatabase implementation
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction( IsolationLevel il )
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
        public ConnectionState State
        {
            get
            {
                return this.cloneableConnection.State;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.cloneableConnection.ConnectionString;
            }

            set
            {
                this.cloneableConnection.ConnectionString = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            IDbCommand command = this.cloneableConnection.CreateCommand();
            return new AsyncDbCommand( this, command );
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
            this.thread.Stop();
            this.thread.Join();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Database
        {
            get
            {
                return this.cloneableConnection.Database;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                return this.cloneableConnection.ConnectionTimeout;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        #endregion

        #region Internal Methods

        internal int ExecuteNonQuery( AsyncDbCommand command )
        {
            string commandText = ToString( command );

            lock (this.commands)
            {
                this.commands.Add( commandText );
            }

            this.queueEvent.Set();

            return 0;
        }

        #endregion

        #region Private Methods

        private static string ToString( IDbCommand command )
        {
            string commandText;

            switch (command.CommandType)
            {
                case CommandType.StoredProcedure:
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat( "exec {0}", command.CommandText );
                    SqlParameterCollection parameters = (SqlParameterCollection) command.Parameters;
                    string parametersString = parameters.ToLogString();

                    if (parametersString.Length > 0)
                    {
                        sb.Append( ' ' );
                        sb.Append( parametersString );
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
                    this.thread.StopRequest,
                    this.queueEvent
                };

            const int Timeout = 10000;// 10 sec

            while (true)
            {
                this.Flush();

                if (this.thread.IsStopRequested)
                {
                    break;
                }

                WaitHandle.WaitAny( waitHandles, Timeout, false );
            }
        }

        private void Flush()
        {
            if (this.commands.Count > 0)
            {
                while (this.commands.Count > 0)
                {
                    string[] commandTextArray;

                    lock (this.commands)
                    {
                        commandTextArray = new string[ this.commands.Count ];
                        this.commands.CopyTo( commandTextArray );
                        this.commands.Clear();
                    }

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < commandTextArray.Length; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append( Environment.NewLine );
                        }

                        sb.Append( commandTextArray[ i ] );
                    }

                    string commandText = sb.ToString();
                    Exception exception = null;

                    using (var connection = (IDbConnection) this.cloneable.Clone())
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
                        SqlException sqlException = exception as SqlException;

                        if (sqlException != null)
                        {
                            message = sqlException.Errors.ToLogString();
                        }
                        else
                        {
                            message = exception.ToLogString();
                        }

                        log.Write( LogLevel.Error, message );
                    }
                }
            }
        }

        #endregion
    }
}