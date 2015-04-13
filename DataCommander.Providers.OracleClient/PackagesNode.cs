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
  class PackagesNode : ITreeNode
  {
    public PackagesNode(SchemaNode schema)
    {
      this.schema = schema;
    }

    public string Name
    {
      get
      {
        return "Packages";
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
//      string commandText = "select object_name from all_objects where owner = '{0}' and object_type = 'PACKAGE' order by object_name";
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
//        treeNodes[i] = new PackageNode(schema,name);
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

    public SchemaNode Schema
    {
      get
      {
        return schema;
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