namespace DataCommander.Providers.Wmi
{
    using System.Data;

    class WmiCommand : IDbCommand
  {
    public WmiCommand(WmiConnection connection)
    {
      Connection = connection;
    }

    public void Dispose()
    {
    }

    public void Cancel()
    {
        Cancelled = true;
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
      get => 0;
            set
      {
      }
    }

    public CommandType CommandType
    {
      get => CommandType.Text;
        set
      {
      }
    }
    
    IDbConnection IDbCommand.Connection
    {
      get => Connection;
        set
      {
      }
    }

    public WmiConnection Connection { get; }

        public IDataParameterCollection Parameters => null;

        public IDbTransaction Transaction
    {
      get => null;
            set
      {
      }
    }
    
    public UpdateRowSource UpdatedRowSource
    {
      get => UpdateRowSource.None;
        set
      {
      }
    }
    
    internal bool Cancelled { get; private set; } = false;
  }
}
