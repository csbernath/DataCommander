using System;
using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ConnectionStateManager : IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ConnectionState _state;

        public ConnectionStateManager( IDbConnection connection )
        {
            this._connection = connection;

            if (connection != null)
            {
                this._state = connection.State;
            }
        }

        public void Open()
        {
            if (this._connection != null && this._state == ConnectionState.Closed)
            {
                this._connection.Open();
            }
        }

        #region IDisposable Member

        public void Dispose()
        {
            if (this._connection != null && this._state == ConnectionState.Closed)
            {
                this._connection.Close();
            }
        }

        #endregion
    }
}