using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Text;
using System.Windows.Forms;

namespace SqlUtil.Providers.OracleClient
{
  /// <summary>
  /// Summary description for TablesNode.
  /// </summary>
  class PackageNode : ITreeNode
  {
    public PackageNode(
      SchemaNode   schema,
      string       name)
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
//      string commandText = "select distinct object_name from all_arguments where owner='{0}' and package_name='{1}'";
//      commandText = String.Format(commandText,schema.Name,name);
//        
//      DataTable dataTable = Core.Oracle.ExecuteSelect(commandText,schema.SchemasNode.Connection);
//      int count = dataTable.Rows.Count;
//
//      ITreeNode[] treeNodes = new ITreeNode[count];
//
//      for (int i=0;i<count;i++)
//      {
//        string procedureName = (string)dataTable.Rows[i][0];
//        treeNodes[i] = new ProcedureNode(this,procedureName);
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
//        string commandText = "select text from all_source where owner = '{0}' and type = 'PACKAGE' and name = '{1}'";
//        commandText = String.Format(commandText,schema.Name,name);
//
//        DataTable dataTable = Core.Oracle.ExecuteSelect(commandText,schema.SchemasNode.Connection);
//        StringBuilder sb = new StringBuilder();
//
//        for (int i=0;i<dataTable.Rows.Count;i++)
//        {
//          DataRow dataRow = dataTable.Rows[i];
//          sb.Append(dataRow[0]);
//        }
//        
//        return sb.ToString();
        return null;
      }
    }

    void ScriptPackage(object sender,EventArgs e)
    {
    }

    void ScriptPackageBody(object sender,EventArgs e)
    {
//      string commandText = "select text from all_source where owner = '{0}' and name = '{1}' and type = 'PACKAGE BODY'";
//      commandText = string.Format(commandText,schema.Name,name);
//
//      DataSet dataSet = new DataSet();
//      Core.OracleHelper.Fill(commandText,schema.SchemasNode.Connection,dataSet);
//      DataRowCollection dataRows = dataSet.Tables[0].Rows;
//      int count = dataRows.Count;
//      StringBuilder sb = new StringBuilder();
//
//      for (int i=0;i<count;i++)
//      {
//        DataRow dataRow = dataRows[i];
//        string line = (string)dataRow[0];
//        sb.Append(line);
//      }
//
//      MainForm mainForm = Application.Instance.MainForm;
//      QueryForm queryForm = (QueryForm)mainForm.ActiveMdiChild;
//      QueryTextBox tbQuery = queryForm.QueryTextBox;
//      int selectionStart = tbQuery.RichTextBox.TextLength;
//
//      string append = sb.ToString();
//
//      tbQuery.RichTextBox.AppendText(append);
//      tbQuery.RichTextBox.SelectionStart = selectionStart;
//      tbQuery.RichTextBox.SelectionLength = append.Length;
//
//      tbQuery.Focus();
    }

    public ContextMenuStrip ContextMenu
    {
      get
      {
        ContextMenuStrip contextMenu = new ContextMenu();

        MenuItem menuItemPackage = new MenuItem("Script Package",new EventHandler(ScriptPackage));
        contextMenu.MenuItems.Add(menuItemPackage);        

        MenuItem menuItemPackageBody = new MenuItem("Script Package Body",new EventHandler(ScriptPackageBody));
        contextMenu.MenuItems.Add(menuItemPackageBody);

        return contextMenu;
      }
    }

    public void BeforeExpand()
    {
    }

    SchemaNode schema;
    string     name;
  }
}
