using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using DataCommander.Application.ResultWriter;
using Foundation.Configuration;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DataCommander.Application.Query;

public sealed partial class QueryForm
{
    private void CloseResultSetTabPage_Click(object sender, EventArgs e)
    {
        var toolStripMenuItem = (ToolStripMenuItem)sender;
        var tabPage = (TabPage)toolStripMenuItem.Tag;
        CloseResultSetTabPage(tabPage);
        toolStripMenuItem.Tag = null;
    }

    private void EditRows_Click(object sender, EventArgs e) => EditRows(Query);
    private void Timer_Tick(object o, EventArgs e) => Invoke(ShowTimer);

    protected override void OnFormClosing(FormClosingEventArgs formClosingEventArgs)
    {
        base.OnFormClosing(formClosingEventArgs);
        var cancel = SaveTextOnFormClosing();
        if (!cancel)
        {
            cancel = CancelQueryOnFormClosing();
            if (!cancel)
            {
                cancel = CommitTransactionOnFormClosing();
                if (!cancel)
                    CloseConnectionOnFormClosing();
            }
        }

        if (cancel)
            formClosingEventArgs.Cancel = cancel;
    }

    private void mnuText_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Text);
    private void mnuDataGrid_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.DataGrid);
    private void mnuHtml_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Html);
    private void mnuRtf_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Rtf);
    private void mnuListView_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.ListView);
    private void mnuExcel_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.Excel);
    private void menuResultModeFile_Click(object sender, EventArgs e) => SetResultWriterType(ResultWriterType.File);

    private void mnuCommandTypeText_Click(object sender, EventArgs e)
    {
        _mnuCommandTypeText.Checked = true;
        _mnuCommandTypeStoredProcedure.Checked = false;
        _commandType = CommandType.Text;
    }

    private void mnuCommandTypeStoredProcedure_Click(object sender, EventArgs e)
    {
        _mnuCommandTypeText.Checked = false;
        _mnuCommandTypeStoredProcedure.Checked = true;
        _commandType = CommandType.StoredProcedure;
    }

    private void menuObjectExplorer_Click(object sender, EventArgs e)
    {
        var visible = !_tvObjectExplorer.Visible;
        _tvObjectExplorer.Visible = visible;
        _splitterObjectExplorer.Visible = visible;
    }

    private void tvObjectBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        var treeNode = e.Node;

        if (treeNode.Nodes.Count > 0)
        {
            var treeNode2 = (ITreeNode)treeNode.Nodes[0].Tag;

            if (treeNode2 == null)
            {
                Cursor = Cursors.WaitCursor;

                try
                {
                    treeNode.Nodes.Clear();
                    treeNode2 = (ITreeNode)treeNode.Tag;
                    IEnumerable<ITreeNode> children = null;

                    try
                    {
                        children = treeNode2.GetChildren(false);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex);
                    }

                    if (children != null)
                        AddNodes(treeNode.Nodes, children, treeNode2.Sortable);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                var count = treeNode.GetNodeCount(false);
                SetStatusbarPanelText(treeNode.Text + " node has " + count + " children.");
            }
        }
    }

    private void tvObjectBrowser_MouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Right:
                var treeNode = _tvObjectExplorer.GetNodeAt(e.X, e.Y);
                if (treeNode != null)
                {
                    var treeNode2 = (ITreeNode)treeNode.Tag;

                    if (e.Button != MouseButtons.Left)
                        _tvObjectExplorer.SelectedNode = treeNode;

                    var text = treeNode.Text;
                }

                break;

            default:
                break;
        }
    }

    private void mnuRefresh_Click(object sender, EventArgs e)
    {
        var treeNodeV = _tvObjectExplorer.SelectedNode;
        if (treeNodeV != null)
        {
            var treeNode = (ITreeNode)treeNodeV.Tag;
            treeNodeV.Nodes.Clear();
            AddNodes(treeNodeV.Nodes, treeNode.GetChildren(true), treeNode.Sortable);
        }
    }

    private void mnuRefreshObjectExplorer_Click(object sender, EventArgs e)
    {
        var objectExplorer = Provider.CreateObjectExplorer();
        if (objectExplorer != null)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var rootNodes = _tvObjectExplorer.Nodes;
                rootNodes.Clear();
                var treeNodes = objectExplorer.GetChildren(true);
                AddNodes(rootNodes, treeNodes, objectExplorer.Sortable);
            }
        }
    }

    private void tvObjectExplorer_MouseUp(object sender, MouseEventArgs e)
    {
        try
        {
            if (e.Button == MouseButtons.Right)
            {
                var treeNodeV = _tvObjectExplorer.SelectedNode;
                if (treeNodeV != null)
                {
                    var treeNode = (ITreeNode)treeNodeV.Tag;
                    var contextMenu = GetContextMenu(treeNode);

                    if (!treeNode.IsLeaf)
                    {
                        if (contextMenu == null)
                            contextMenu = new ContextMenuStrip(components);

                        contextMenu.Items.Add(new ToolStripMenuItem("Refresh", null, mnuRefresh_Click));
                    }

                    if (contextMenu != null)
                    {
                        if (_colorTheme != null)
                        {
                            contextMenu.ForeColor = _colorTheme.ForeColor;
                            contextMenu.BackColor = _colorTheme.BackColor;
                        }

                        var contains = components.Components.Cast<IComponent>().Contains(contextMenu);
                        if (!contains)
                        {
                            components.Add(contextMenu);
                            GarbageMonitor.Default.Add("contextMenu", contextMenu);
                        }

                        var pos = new Point(e.X, e.Y);
                        contextMenu.Show(_tvObjectExplorer, pos);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private void mnuPaste_Click(object sender, EventArgs e) => QueryTextBox.Paste();

    private void mnuGoTo_Click(object sender, EventArgs e)
    {
        var control = ActiveControl;
        var richTextBox = control as RichTextBox;
        if (richTextBox == null)
            richTextBox = QueryTextBox.RichTextBox;

        var charIndex = richTextBox.SelectionStart;
        var currentLineNumber = richTextBox.GetLineFromCharIndex(charIndex) + 1;
        var form = new GotoLineForm();
        var maxLineNumber = richTextBox.Lines.Length;
        form.Init(currentLineNumber, maxLineNumber);

        if (form.ShowDialog(this) == DialogResult.OK)
        {
            var lineNumber = form.LineNumber;
            charIndex = NativeMethods.SendMessage(richTextBox.Handle.ToInt32(), (int)NativeMethods.Message.EditBox.LineIndex, lineNumber - 1, 0);
            richTextBox.SelectionStart = charIndex;
        }
    }

    private void mnuFind_Click(object sender, EventArgs e)
    {
        try
        {
            if (_findTextForm == null)
                _findTextForm = new FindTextForm();

            var control = ActiveControl;
            var dataTableViewer = control as DataTableEditor;

            if (dataTableViewer == null)
            {
                control = control.Parent;
                dataTableViewer = control as DataTableEditor;
            }

            if (dataTableViewer != null)
            {
                var dataTable = dataTableViewer.DataTable;
                var name = dataTable.TableName;
                _findTextForm.Text = $"Find (DataTable: {name})";
            }
            else
                _findTextForm.Text = "Find";

            if (_findTextForm.ShowDialog() == DialogResult.OK)
                FindText(_findTextForm.FindText);
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuFindNext_Click(object sender, EventArgs e)
    {
        if (_findTextForm != null)
        {
            var text = _findTextForm.FindText;
            if (text != null)
                FindText(text);
        }
    }

    private void mnuSave_Click(object sender, EventArgs e)
    {
        Save();
    }

    private void mnuSaveAs_Click(object sender, EventArgs e)
    {
        ShowSaveFileDialog();
    }

    private void mnuGotoQueryEditor_Click(object sender, EventArgs e)
    {
        QueryTextBox.Select();
    }

    private void mnuGotoMessageTabPage_Click(object sender, EventArgs e)
    {
        _tabControl.SelectedTab = _messagesTabPage;
    }

    private void mnuListMembers_Click(object sender, EventArgs e)
    {
        if (QueryTextBox.KeyboardHandler == null)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                var response = GetCompletion();
                if (response.Items != null)
                {
                    var completionForm = new CompletionForm(this);
                    completionForm.Initialize(QueryTextBox, response, _colorTheme);
                    completionForm.ItemSelected += completionForm_ItemSelected;
                    completionForm.Show(this);
                    QueryTextBox.RichTextBox.Focus();
                }
            }
        }
    }

    private void completionForm_ItemSelected(object sender, ItemSelectedEventArgs e)
    {
        var textBox = QueryTextBox;

        var intPtr = textBox.RichTextBox.Handle;
        var hWnd = intPtr.ToInt32();
        NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

        var objectName = e.ObjectName.QuotedName;

        textBox.RichTextBox.SelectionStart = e.StartIndex;
        textBox.RichTextBox.SelectionLength = e.Length;
        textBox.RichTextBox.SelectedText = objectName;
        textBox.RichTextBox.SelectionStart = e.StartIndex + objectName.Length;

        NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 1, 0);
    }

    private void mnuClearCache_Click(object sender, EventArgs e)
    {
        Provider.ClearCompletionCache();
    }

    private void mnuResultSchema_Click(object sender, EventArgs e)
    {
        ExecuteReader(CommandBehavior.SchemaOnly);
    }

    private void mnuKeyInfo_Click(object sender, EventArgs e)
    {
        ExecuteReader(CommandBehavior.KeyInfo);
    }

    private void mnuSingleRow_Click(object sender, EventArgs e)
    {
        ExecuteQuerySingleRow();
    }

    private void mnuShowShemaTable_Click(object sender, EventArgs e)
    {
        _mnuShowShemaTable.Checked = !_mnuShowShemaTable.Checked;
        _showSchemaTable = !_showSchemaTable;
    }

    private void mnuXml_Click(object sender, EventArgs e)
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

            using (var dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    var writer = new StringWriter();
                    var read = false;

                    while (dataReader.Read())
                    {
                        if (!read)
                        {
                            read = true;
                        }

                        var fragment = (string)dataReader[0];
                        writer.Write(fragment);
                    }

                    if (read)
                    {
                        var xml = writer.ToString();
                        var xmlDocument = new XmlDocument();
                        var path = Path.GetTempFileName() + ".xml";

                        try
                        {
                            xmlDocument.LoadXml(xml);
                            xmlDocument.Save(path);
                        }
                        catch
                        {
                            var xmlWriter = new XmlTextWriter(path, Encoding.UTF8);
                            xmlWriter.WriteStartElement("DataCommanderRoot");
                            xmlWriter.WriteRaw(xml);
                            xmlWriter.WriteEndElement();
                            xmlWriter.Close();
                        }

                        var resultSetTabPage = new TabPage("Xml");
                        _resultSetsTabControl.TabPages.Add(resultSetTabPage);

                        var htmlTextBox = new HtmlTextBox();
                        htmlTextBox.Dock = DockStyle.Fill;

                        resultSetTabPage.Controls.Add(htmlTextBox);

                        htmlTextBox.Navigate(path);
                        _resultSetsTabControl.SelectedTab = resultSetTabPage;
                        _tabControl.SelectedTab = _resultSetsTabPage;
                    }

                    if (!dataReader.NextResult())
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuCreateInsert_Click(object sender, EventArgs e)
    {
        try
        {
            var sqlKeyWords = Settings.CurrentType.Attributes["Sql92ReservedWords"].GetValue<string[]>();
            var providerKeyWords = Provider.KeyWords;
            var keyWordHashSet = sqlKeyWords.Concat(providerKeyWords)
                .Select(keyWord => keyWord.ToUpper())
                .ToHashSet();

            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, CommandType.Text, _commandTimeout);
            var tableName = _sqlStatement.FindTableName();
            var tableIndex = 0;

            using (var dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    if (tableIndex > 0)
                    {
                        tableName = $"Table{tableIndex}";
                    }

                    var dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    var schemaTable = dataReader.GetSchemaTable();
                    var sb = new StringBuilder();

                    if (schemaTable != null)
                    {
                        if (tableName != null)
                            schemaTable.TableName = tableName;
                        else
                            tableName = schemaTable.TableName;

                        _standardOutput.WriteLine(InsertScriptFileWriter.GetCreateTableStatement(schemaTable));
                        var schemaRows = schemaTable.Rows;
                        var columnCount = schemaRows.Count;
                        sb.AppendFormat("insert into {0}(", tableName);

                        for (var i = 0; i < columnCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            var schemaRow = schemaRows[i];
                            var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];

                            if (keyWordHashSet.Contains((columnName.ToUpper())))
                                columnName = new SqlCommandBuilder().QuoteIdentifier(columnName);

                            sb.Append(columnName);
                        }
                    }

                    sb.Append(") values(");
                    var insertInto = sb.ToString();
                    var fieldCount = dataReader.FieldCount;
                    sb.Length = 0;
                    var statementCount = 0;

                    while (dataReader.Read())
                    {
                        var values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb.Append(insertInto);

                        for (var i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            var s = InsertScriptFileWriter.ToString(values[i]);
                            sb.Append(s);
                        }

                        sb.AppendLine(");");
                        ++statementCount;

                        if (statementCount % 100 == 0)
                        {
                            _standardOutput.Write(sb);
                            sb.Length = 0;
                        }
                    }

                    if (statementCount % 100 != 0)
                        _standardOutput.Write(sb);

                    if (!dataReader.NextResult())
                    {
                        break;
                    }

                    tableIndex++;
                }
            }

            _tabControl.SelectedTab = _messagesTabPage;
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void mnuCreateInsertSelect_Click(object sender, EventArgs e)
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, CommandType.Text, _commandTimeout);
            var tableName = _sqlStatement.FindTableName();

            if (tableName != null)
            {
                using (var dataReader = _command.ExecuteReader())
                {
                    var dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    var schemaTable = dataReader.GetSchemaTable();
                    var schemaRows = schemaTable.Rows;
                    var columnCount = schemaRows.Count;
                    var sb = new StringBuilder();
                    sb.AppendFormat("insert into {0}(", tableName);

                    for (var i = 0; i < columnCount; ++i)
                    {
                        if (i > 0)
                            sb.Append(',');

                        var schemaRow = schemaRows[i];
                        var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                        sb.Append(columnName);
                    }

                    sb.Append(")\r\nselect\r\n");
                    var insertInto = sb.ToString();
                    var fieldCount = dataReader.FieldCount;

                    while (dataReader.Read())
                    {
                        var values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb = new StringBuilder();
                        sb.Append(insertInto);

                        for (var i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(",\r\n");

                            var s = InsertScriptFileWriter.ToString(values[i]);
                            sb.AppendFormat("    {0} as {1}", s, dataReader.GetName(i));
                        }

                        _standardOutput.WriteLine(sb);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void tvObjectBrowser_ItemDrag(object sender, ItemDragEventArgs e)
    {
        var treeNode = (TreeNode)e.Item;
        var text = treeNode.Text;
        _tvObjectExplorer.DoDragDrop(text, DragDropEffects.All);
    }

    private async void mnuDuplicateConnection_Click(object sender, EventArgs e)
    {
        var mainForm = DataCommanderApplication.Instance.MainForm;
        var index = mainForm.MdiChildren.Length;

        var connection = Provider.CreateConnection(_connectionString);
        connection.ConnectionName = Connection.ConnectionName;
        await connection.OpenAsync(CancellationToken.None);
        var database = Connection.Database;

        if (connection.Database != Connection.Database)
            connection.Connection.ChangeDatabase(database);

        var queryForm = new QueryForm(_mainForm, Provider, _connectionString, connection, mainForm.StatusBar, _colorTheme);

        if (mainForm.SelectedFont != null)
            queryForm.Font = mainForm.SelectedFont;

        queryForm.MdiParent = mainForm;
        queryForm.WindowState = WindowState;
        queryForm.Show();
    }

    private void sQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SetResultWriterType(ResultWriterType.SqLite);
    }

    private void createSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var sqlStatement = new SqlParser(Query);
        _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
        var maxRecords = int.MaxValue;
        var tableName = sqlStatement.FindTableName();
        var sqlCeResultWriter = new SqlCeResultWriter(_textBoxWriter, tableName);
        IAsyncDataAdapter asyncDataAdatper = new AsyncDataAdapter(
            Provider,
            new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray(),
            maxRecords, _rowBlockSize, sqlCeResultWriter, EndFillInvoker, WriteEndInvoker);
        asyncDataAdatper.Start();
    }

    private void beginTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction == null)
        {
            var transaction = Connection.Connection.BeginTransaction();
            SetTransaction(transaction);
        }
    }

    private void commitTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;

            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction commited successfully."));

            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = true;
            _commitTransactionToolStripMenuItem.Enabled = false;
            _rollbackTransactionToolStripMenuItem.Enabled = false;
        }
    }

    private void rollbackTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction != null)
        {
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction rolled back successfully."));
            }
            catch (Exception ex)
            {
                var message = $"Rollback failed. Exception:\r\n{ex.ToLogString()}";
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Error, null, message));
            }

            _transaction = null;
            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = true;
            _commitTransactionToolStripMenuItem.Enabled = false;
            _rollbackTransactionToolStripMenuItem.Enabled = false;
        }
    }

    private void insertScriptFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SetResultWriterType(ResultWriterType.InsertScriptFile);
    }
}