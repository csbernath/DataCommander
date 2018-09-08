using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  /// <summary>
  /// Summary description for SchemaNode.
  /// </summary>
  class SchemaNode : ITreeNode
  {
    public SchemaNode(
      SchemasNode schemasNode,
      string      name)
    {
      this.schemasNode = schemasNode;
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
      ITreeNode[] treeNodes = new ITreeNode[] {new TablesNode(this),new ViewsNode(this),new PackagesNode(this)};
      return treeNodes;
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
      schemasNode.SelectedSchema = name;
    }

    public SchemasNode SchemasNode
    {
      get
      {
        return schemasNode;
      }
    }

    SchemasNode schemasNode;
    string      name;
  }
}
