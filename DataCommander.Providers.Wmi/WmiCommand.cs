namespace DataCommander.Providers.Wmi
{
    using System.Data;

    class WmiCommand : IDbCommand
  {
    public WmiCommand(WmiConnection connection)
    {
      this.Connection = connection;
    }

    public void Dispose()
    {
    }

    public void Cancel()
    {
        this.Cancelled = true;
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

    public string CommandText { get; set; }

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
        return this.Connection;
      }
      set
      {
      }
    }

    public WmiConnection Connection { get; }

        public IDataParameterCollection Parameters => null;

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
    
    internal bool Cancelled { get; private set; } = false;
  }
}
