namespace DataCommander.Providers.Wmi
{
    using System.Data;

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
        this.cancelled = true;
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
        return this.commandText;
      }
      set
      {
          this.commandText = value;
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
        return this.connection;
      }
      set
      {
      }
    }

    public WmiConnection Connection => this.connection;

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
    
    internal bool Cancelled => this.cancelled;

        readonly WmiConnection connection;
    string        commandText;
    bool          cancelled = false;
  }
}
