using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  class ProcedureNode : ITreeNode
  {
    public ProcedureNode(
      PackageNode parent,
      string      name)
    {
      this.parent = parent;
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
        return true;
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
        string packageName = parent.Name;
        string query = "EXEC " + packageName + "." + name;
        return query;
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

    PackageNode parent;
    string      name;
  }
}