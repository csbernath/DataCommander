using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  /// <summary>
  /// Summary description for SchemaNode.
  /// </summary>
  class SchemasNode : ITreeNode
  {
    public SchemasNode(OracleConnection connection)
    {
      this.connection = connection;
      this.selectedSchema = connection.DataSource;
    }

    public string Name
    {
      get
      {
        return "Schemas";
      }
    }
    
    public bool IsLeaf
    {
      get
      {
        return false;
      }
    }
    
    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
//      string commandText = "select username from all_users order by username";
//      DataTable dataTable = Core.Oracle.ExecuteSelect(commandText,connection);
//      int count = dataTable.Rows.Count;
//
//      ITreeNode[] treeNodes = new ITreeNode[count];
//
//      for (int i=0;i<count;i++)
//      {
//        string name = (string)dataTable.Rows[i][0];
//        treeNodes[i] = new SchemaNode(this,name);
//      }
//
//      return treeNodes;
      return null;
    }

    public bool Sortable
    {
      get
      {
        return false;
      }
    }

    public string Query
    {
      get
      {
        return null;
      }
    }

    public ContextMenuStrip ContextMenu
    {
      get
      {
        return null;
      }
    }

    public OracleConnection Connection
    {
      get
      {
        return connection;
      }
    }

    public void BeforeExpand()
    {
    }

    public string SelectedSchema
    {
      get
      {
        return selectedSchema;
      }
      set
      {
        selectedSchema = value;
      }
    }

    OracleConnection connection;
    string           selectedSchema;
  }
}
