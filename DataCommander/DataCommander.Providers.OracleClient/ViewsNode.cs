using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  class ViewsNode : ITreeNode
  {
    public ViewsNode(SchemaNode schemaNode)
    {
      this.schemaNode = schemaNode;
    }

    public string Name
    {
      get
      {
        return "Views";
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
//      string commandText = "select view_name from all_views where owner = '{0}'";
//      commandText = String.Format(commandText,schemaNode.Name);
//      DataTable dataTable = WAVE.Foundation.Data.Oracle.ExecuteSelect(commandText,schemaNode.SchemasNode.Connection);
//      DataRowCollection dataRows = dataTable.Rows;
//      int count = dataRows.Count;
//      ITreeNode[] treeNodes = new ITreeNode[count];
//
//      for (int i=0;i<count;i++)
//      {
//        string name = (string)dataRows[i][0];
//        treeNodes[i] = new ViewNode(this,name);
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

    public SchemaNode SchemaNode
    {
      get
      {
        return schemaNode;
      }
    }

    public void BeforeExpand()
    {
    }

    SchemaNode schemaNode;
  }
}