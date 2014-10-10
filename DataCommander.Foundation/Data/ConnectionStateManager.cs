namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ConnectionStateManager : IDisposable
    {
        private readonly IDbConnection connection;
        private readonly ConnectionState state;

        public ConnectionStateManager( IDbConnection connection )
        {
            this.connection = connection;

            if (connection != null)
            {
                this.state = connection.State;
            }
        }

        public void Open()
        {
            if (this.connection != null && this.state == ConnectionState.Closed)
            {
                this.connection.Open();
            }
        }

        #region IDisposable Member

        public void Dispose()
        {
            if (this.connection != null && this.state == ConnectionState.Closed)
            {
                this.connection.Close();
            }
        }

        #endregion
    }
}