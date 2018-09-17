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
  class TableNode : ITreeNode
  {
    public TableNode(
      SchemaNode schema,
      string     name)
    {
      this.schema = schema;
      this.name = name;
    }

    public string Name
    {
      get
      {
        return name;
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
        return "select * from " + schema.Name + "." + name;
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
    string     name;
  }
}
