using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SqlUtil.Providers;

namespace SqlUtil.Providers.OracleClient
{
  class ViewNode : ITreeNode
  {
    public ViewNode(ViewsNode parent,string name)
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
        string query = string.Format("select * from {0}.{1}",parent.SchemaNode.Name,name);
        return query;
      }    
    }

    void menuItemScriptObject_Click(object sender,EventArgs e)
    {                  
      //Scripter.SpHelpText(name);
    }

    public ContextMenuStrip ContextMenu
    {
      get
      {
//        MenuItem menuItemScriptObject = new MenuItem("Script Object",new EventHandler(menuItemScriptObject_Click));
//        ContextMenu contextMenu = new ContextMenu();
//        contextMenu.MenuItems.Add(menuItemScriptObject);
//        return contextMenu;

        return null;
      }
    }

    public void BeforeExpand()
    {
    }

    ViewsNode parent;
    string    name;
  }
}
