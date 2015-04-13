using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  /// <summary>
  /// Summary description for TablesNode.
  /// </summary>
  class TablesNode : ITreeNode
  {
    public TablesNode(SchemaNode schema)
    {
      this.schema = schema;
    }

    public string Name
    {
      get
      {
        return "Tables";
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
//      string commandText = "select table_name from all_tables where owner = '{0}'";
//      commandText = String.Format(commandText,schema.Name);
//        
//      DataTable dataTable = Core.Oracle.ExecuteSelect(commandText,schema.SchemasNode.Connection);
//      int count = dataTable.Rows.Count;
//
//      ITreeNode[] treeNodes = new ITreeNode[count];
//
//      for (int i=0;i<count;i++)
//      {
//        string name = (string)dataTable.Rows[i][0];
//        treeNodes[i] = new TableNode(schema,name);
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

    public void BeforeExpand()
    {
    }

    SchemaNode schema;
  }
}
