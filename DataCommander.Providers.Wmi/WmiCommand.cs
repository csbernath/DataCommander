using System.Data;

namespace DataCommander.Providers.Wmi
{
  class WmiCommand : IDbCommand
  {
    public WmiCommand(WmiConnection connection)
    {
      this.connection = connection;
    }

    public void Dispose()
    {
    }

    public void Cancel()
    {
      cancelled = true;
    }

    public IDbDataParameter CreateParameter()
    {
      return null;
    }

    public int ExecuteNonQuery()
    {
      return 0;
    }

    public IDataReader ExecuteReader()
    {
      return new WmiDataReader(this);
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
      return new WmiDataReader(this);
    }

    public object ExecuteScalar()
    {
      return null;
    }

    public void Prepare()
    {
    }

    public string CommandText
    {
      get
      {
        return commandText;
      }
      set
      {
        commandText = value;
      }
    }
    public int CommandTimeout
    {
      get
      {
        return 0;
      }      
      set
      {
      }
    }

    public CommandType CommandType
    {
      get
      {
        return CommandType.Text;
      }      
      set
      {
      }
    }
    
    IDbConnection IDbCommand.Connection
    {
      get
      {
        return connection;
      }
      set
      {
      }
    }

    public WmiConnection Connection
    {
      get
      {
        return connection;
      }
    }
    
    public IDataParameterCollection Parameters
    {
      get
      {
        return null;
      }
    }
    
    public IDbTransaction Transaction
    {
      get
      {
        return null;
      }
      set
      {
      }
    }
    
    public UpdateRowSource UpdatedRowSource
    {
      get
      {
        return UpdateRowSource.None;
      }
      set
      {
      }
    }
    
    internal bool Cancelled
    {
      get
      {
        return cancelled;
      }
    }

    WmiConnection connection;
    string        commandText;
    bool          cancelled = false;
  }
}
