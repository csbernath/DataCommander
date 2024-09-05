using System;
using System.Data;

namespace Foundation.Data;

internal sealed class ConnectionStateManager : IDisposable
{
    private readonly IDbConnection _connection;
    private readonly ConnectionState _state;

    public ConnectionStateManager(IDbConnection connection)
    {
        _connection = connection;

        if (connection != null)
            _state = connection.State;
    }

    public void Open()
    {
        if (_connection != null && _state == ConnectionState.Closed)
            _connection.Open();
    }

    public void Dispose()
    {
        if (_connection != null && _state == ConnectionState.Closed)
        {
            _connection.Close();
        }
    }
}