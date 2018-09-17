using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using SqlUtil.Providers;

namespace SqlUtil.Providers.OracleClient
{
	/// <summary>
	/// Summary description for ObjectBrowser.
	/// </summary>
	class ObjectExplorer : IObjectExplorer
	{
		public ObjectExplorer()
		{
		}

    public IDbConnection Connection
    {
      set
      {
        this.connection = (OracleConnection)value;
        schemasNode = new SchemasNode(connection);
      }
    }

    public OracleConnection OracleConnection
    {
      get
      {
        return connection;
      }
    }

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
      return new ITreeNode[] {schemasNode};
    }

    public bool Sortable
    {
      get
      {
        return false;
      }
    }

    public SchemasNode SchemasNode
    {
      get
      {
        return schemasNode;
      }
    }

    OracleConnection connection;
    SchemasNode      schemasNode;
	}
}
