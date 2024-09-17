using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ADODB;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using DataCommander.Application.ResultWriter;
using Foundation.Configuration;
using Foundation.Core;
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
        ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
        TabPage? tabPage = (TabPage)toolStripMenuItem.Tag;
        CloseResultSetTabPage(tabPage);
        toolStripMenuItem.Tag = null;
    }

    private void EditRows_Click(object sender, EventArgs e) => EditRows(Query);
    private void Timer_Tick(object o, EventArgs e) => Invoke(ShowTimer);

    protected override void OnFormClosing(FormClosingEventArgs formClosingEventArgs)
    {
        using (new CursorManager(Cursors.WaitCursor))
        {
            base.OnFormClosing(formClosingEventArgs);
            bool cancel = SaveTextOnFormClosing();
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
        bool visible = !_tvObjectExplorer.Visible;
        _tvObjectExplorer.Visible = visible;
        _splitterObjectExplorer.Visible = visible;
    }

    private void tvObjectBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        TreeNode? treeNode = e.Node;

        if (treeNode.Nodes.Count > 0)
        {
            ITreeNode? treeNode2 = (ITreeNode)treeNode.Nodes[0].Tag;

            if (treeNode2 == null)
            {
                Cursor = Cursors.WaitCursor;

                try
                {
                    try
                    {
                        long startTimestamp = Stopwatch.GetTimestamp();
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        CancellationToken cancellationToken = cancellationTokenSource.Token;
                        treeNode2 = (ITreeNode)treeNode.Tag;
                        CancelableOperationForm cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1),
                            "Getting tree node children...",
                            $@"Parent node type: {treeNode2.GetType().Name}
Parent node name: {treeNode2.Name}
Please wait...",
                            _colorTheme);
                        IEnumerable<ITreeNode> children = cancelableOperationForm.Execute(new Task<IEnumerable<ITreeNode>>(() =>
                            treeNode2.GetChildren(false, cancellationToken).Result));
                        treeNode.Nodes.Clear();
                        AddNodes(treeNode.Nodes, children, treeNode2.Sortable, startTimestamp);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex);
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else
            {
                int count = treeNode.GetNodeCount(false);
                SetStatusbarPanelText(treeNode.Text + " node has " + count + " children.");
            }
        }
    }

    private void TvObjectBrowser_MouseDown(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Right:
                TreeNode? treeNode = _tvObjectExplorer.GetNodeAt(e.X, e.Y);
                if (treeNode != null)
                {
                    ITreeNode? treeNode2 = (ITreeNode)treeNode.Tag;

                    if (e.Button != MouseButtons.Left)
                        _tvObjectExplorer.SelectedNode = treeNode;

                    string text = treeNode.Text;
                }

                break;

            default:
                break;
        }
    }

    private void MnuRefresh_Click(object sender, EventArgs e)
    {
        TreeNode? treeNodeV = _tvObjectExplorer.SelectedNode;
        if (treeNodeV != null)
        {
            ITreeNode? treeNode = (ITreeNode)treeNodeV.Tag;
            treeNodeV.Nodes.Clear();

            long startTimestamp = Stopwatch.GetTimestamp();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            CancelableOperationForm cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1),
                "Getting tree node children...", "Please wait...", _colorTheme);
            IEnumerable<ITreeNode> children = cancelableOperationForm.Execute(new Task<IEnumerable<ITreeNode>>(() => treeNode.GetChildren(true, cancellationToken).Result));
            AddNodes(treeNodeV.Nodes, children, treeNode.Sortable, startTimestamp);
        }
    }

    private void MnuRefreshObjectExplorer_Click(object sender, EventArgs e)
    {
        IObjectExplorer objectExplorer = Provider.CreateObjectExplorer();
        if (objectExplorer != null)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                TreeNodeCollection rootNodes = _tvObjectExplorer.Nodes;
                rootNodes.Clear();
                long startTimestamp = Stopwatch.GetTimestamp();
                IEnumerable<ITreeNode> treeNodes = objectExplorer.GetChildren(true, CancellationToken.None).Result;
                AddNodes(rootNodes, treeNodes, objectExplorer.Sortable, startTimestamp);
            }
        }
    }

    private void tvObjectExplorer_MouseUp(object sender, MouseEventArgs e)
    {
        try
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode? treeNodeV = _tvObjectExplorer.SelectedNode;
                if (treeNodeV != null)
                {
                    ITreeNode? treeNode = (ITreeNode)treeNodeV.Tag;
                    ContextMenuStrip? contextMenu = GetContextMenu(treeNode);

                    if (!treeNode.IsLeaf)
                    {
                        if (contextMenu == null)
                            contextMenu = new ContextMenuStrip(components);

                        contextMenu.Items.Add(new ToolStripMenuItem("Refresh", null, MnuRefresh_Click));
                    }

                    if (contextMenu != null)
                    {
                        if (_colorTheme != null)
                        {
                            contextMenu.ForeColor = _colorTheme.ForeColor;
                            contextMenu.BackColor = _colorTheme.BackColor;
                        }

                        bool contains = components.Components.Cast<IComponent>().Contains(contextMenu);
                        if (!contains)
                        {
                            components.Add(contextMenu);
                            GarbageMonitor.Default.Add("contextMenu", contextMenu);
                        }

                        Point pos = new Point(e.X, e.Y);
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
        Control? control = ActiveControl;
        if (control is not RichTextBox richTextBox)
            richTextBox = QueryTextBox.RichTextBox;

        int charIndex = richTextBox.SelectionStart;
        int currentLineNumber = richTextBox.GetLineFromCharIndex(charIndex) + 1;
        GotoLineForm form = new GotoLineForm();
        int maxLineNumber = richTextBox.Lines.Length;
        form.Init(currentLineNumber, maxLineNumber);

        if (form.ShowDialog(this) == DialogResult.OK)
        {
            int lineNumber = form.LineNumber;
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

            Control? control = ActiveControl;
            DataTableEditor? dataTableViewer = control as DataTableEditor;

            if (dataTableViewer == null)
            {
                control = control.Parent;
                dataTableViewer = control as DataTableEditor;
            }

            if (dataTableViewer != null)
            {
                DataTable dataTable = dataTableViewer.DataTable;
                string name = dataTable.TableName;
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
            string text = _findTextForm.FindText;
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
                GetCompletionResult response = GetCompletion();
                if (response.Items != null)
                {
                    CompletionForm completionForm = new CompletionForm(this);
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
        QueryTextBox textBox = QueryTextBox;

        nint intPtr = textBox.RichTextBox.Handle;
        int hWnd = intPtr.ToInt32();
        NativeMethods.SendMessage(hWnd, (int)NativeMethods.Message.Gdi.SetRedraw, 0, 0);

        string objectName = e.ObjectName.QuotedName;

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

            using (DbDataReader dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    StringWriter writer = new StringWriter();
                    bool read = false;

                    while (dataReader.Read())
                    {
                        if (!read)
                        {
                            read = true;
                        }

                        string fragment = (string)dataReader[0];
                        writer.Write(fragment);
                    }

                    if (read)
                    {
                        string xml = writer.ToString();
                        XmlDocument xmlDocument = new XmlDocument();
                        string path = Path.GetTempFileName() + ".xml";

                        try
                        {
                            xmlDocument.LoadXml(xml);
                            xmlDocument.Save(path);
                        }
                        catch
                        {
                            XmlTextWriter xmlWriter = new XmlTextWriter(path, Encoding.UTF8);
                            xmlWriter.WriteStartElement("DataCommanderRoot");
                            xmlWriter.WriteRaw(xml);
                            xmlWriter.WriteEndElement();
                            xmlWriter.Close();
                        }

                        TabPage resultSetTabPage = new TabPage("Xml");
                        _resultSetsTabControl.TabPages.Add(resultSetTabPage);

                        HtmlTextBox htmlTextBox = new HtmlTextBox
                        {
                            Dock = DockStyle.Fill
                        };

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
            string[] sqlKeyWords = Settings.CurrentType.Attributes["SqlReservedWords"].GetValue<string[]>();
            string[] providerKeyWords = Provider.KeyWords;
            HashSet<string> keyWordHashSet = sqlKeyWords.Concat(providerKeyWords)
                .Select(keyWord => keyWord.ToUpper())
                .ToHashSet();

            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, CommandType.Text, _commandTimeout);
            string? tableName = _sqlStatement.FindTableName();
            int tableIndex = 0;

            using (DbDataReader dataReader = _command.ExecuteReader())
            {
                while (true)
                {
                    if (tableIndex > 0)
                    {
                        tableName = $"Table{tableIndex}";
                    }

                    IDataReaderHelper dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    DataTable? schemaTable = dataReader.GetSchemaTable();
                    StringBuilder sb = new StringBuilder();

                    if (schemaTable != null)
                    {
                        if (tableName != null)
                            schemaTable.TableName = tableName;
                        else
                            tableName = schemaTable.TableName;

                        _standardOutput.WriteLine(InsertScriptFileWriter.GetCreateTableStatement(schemaTable));
                        DataRowCollection schemaRows = schemaTable.Rows;
                        int columnCount = schemaRows.Count;
                        sb.AppendFormat("insert into {0}(", tableName);

                        for (int i = 0; i < columnCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            DataRow schemaRow = schemaRows[i];
                            string columnName = (string)schemaRow[SchemaTableColumn.ColumnName];

                            if (keyWordHashSet.Contains((columnName.ToUpper())))
                                columnName = new SqlCommandBuilder().QuoteIdentifier(columnName);

                            sb.Append(columnName);
                        }
                    }

                    sb.Append(") values(");
                    string insertInto = sb.ToString();
                    int fieldCount = dataReader.FieldCount;
                    sb.Length = 0;
                    int statementCount = 0;

                    while (dataReader.Read())
                    {
                        object[] values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb.Append(insertInto);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            string s = InsertScriptFileWriter.ToString(values[i]);
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
            string? tableName = _sqlStatement.FindTableName();

            if (tableName != null)
            {
                using (DbDataReader dataReader = _command.ExecuteReader())
                {
                    IDataReaderHelper dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    DataTable? schemaTable = dataReader.GetSchemaTable();
                    DataRowCollection schemaRows = schemaTable.Rows;
                    int columnCount = schemaRows.Count;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("insert into {0}(", tableName);

                    for (int i = 0; i < columnCount; ++i)
                    {
                        if (i > 0)
                            sb.Append(',');

                        DataRow schemaRow = schemaRows[i];
                        string columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                        sb.Append(columnName);
                    }

                    sb.Append(")\r\nselect\r\n");
                    string insertInto = sb.ToString();
                    int fieldCount = dataReader.FieldCount;

                    while (dataReader.Read())
                    {
                        object[] values = new object[fieldCount];
                        dataReaderHelper.GetValues(values);
                        sb = new StringBuilder();
                        sb.Append(insertInto);

                        for (int i = 0; i < fieldCount; i++)
                        {
                            if (i > 0)
                                sb.Append(",\r\n");

                            string s = InsertScriptFileWriter.ToString(values[i]);
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
        TreeNode? treeNode = (TreeNode)e.Item;
        string text = treeNode.Text;
        _tvObjectExplorer.DoDragDrop(text, DragDropEffects.All);
    }

    private async void mnuDuplicateConnection_Click(object sender, EventArgs e)
    {
        MainForm mainForm = DataCommanderApplication.Instance.MainForm;
        int index = mainForm.MdiChildren.Length;

        ConnectionBase connection = Provider.CreateConnection(_connectionInfo.ConnectionStringAndCredential);
        //connection.ConnectionName = Connection.ConnectionName;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        CancelableOperationForm cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1),
            "Opening connection...", string.Empty, _colorTheme);
        Stopwatch stopwatch = Stopwatch.StartNew();
        cancelableOperationForm.Execute(new Task(() => connection.OpenAsync(cancellationToken).Wait(cancellationToken)));
        long elapsedTicks = stopwatch.ElapsedTicks;

        string? database = Connection.Database;

        if (connection.Database != Connection.Database)
            connection.Connection.ChangeDatabase(database);

        QueryForm queryForm = new QueryForm(_mainForm, Provider, _connectionInfo, connection, mainForm.StatusBar, _colorTheme);

        if (mainForm.SelectedFont != null)
            queryForm.Font = mainForm.SelectedFont;

        queryForm.MdiParent = mainForm;
        queryForm.WindowState = WindowState;
        queryForm.Show();

        ProviderInfo providerInfo = ProviderInfoRepository.GetProviderInfos().First(i => i.Identifier == _connectionInfo.ProviderIdentifier);
        QueryFormStaticMethods.AddInfoMessageToQueryForm(queryForm, elapsedTicks, _connectionInfo.ConnectionName, providerInfo.Name, connection);
    }

    private void sQLiteDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SetResultWriterType(ResultWriterType.SqLite);
    }

    private void createSqlCeDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SqlParser sqlStatement = new SqlParser(Query);
        _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
        int maxRecords = int.MaxValue;
        string? tableName = sqlStatement.FindTableName();
        SqlCeResultWriter sqlCeResultWriter = new SqlCeResultWriter(_textBoxWriter, tableName);
        AsyncDataAdapter asyncDataAdatper = new AsyncDataAdapter(Provider, maxRecords, _rowBlockSize, sqlCeResultWriter, EndFillInvoker, WriteEndInvoker);
        asyncDataAdatper.Start(new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray());
    }

    private void beginTransactionToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_transaction == null)
        {
            DbTransaction transaction = Connection.Connection.BeginTransaction();
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
                string message = $"Rollback failed. Exception:\r\n{ex.ToLogString()}";
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

    private void DataTableTabControl_MouseUp(object? sender, MouseEventArgs e)
    {
        if (sender != null && e.Button == MouseButtons.Middle)
        {
            TabControl? tabControl = (TabControl)sender;
            Tchittestinfo hitTestInfo = new Tchittestinfo(e.X, e.Y);
            int index = SendMessage(tabControl.Handle, TcmHittest, IntPtr.Zero, ref hitTestInfo);
            if (index >= 0)
            {
                tabControl.TabPages.RemoveAt(index);

                if (tabControl.TabPages.Count == 0)
                {
                    TabPage? tabPage = (TabPage)tabControl.Parent;
                    tabControl = (TabControl)tabPage.Parent;
                    tabControl.TabPages.Remove(tabPage);
                }
            }
        }
    }

    private void Connection_InfoMessage(IReadOnlyCollection<InfoMessage> messages) => AddInfoMessages(messages);

    private void sbPanelTableStyle_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip(components);
            Array values = Enum.GetValues(typeof(ResultWriterType));

            for (int i = 0; i < values.Length; i++)
            {
                ResultWriterType tableStyle = (ResultWriterType)values.GetValue(i);
                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = tableStyle.ToString(),
                    Tag = tableStyle
                };
                item.Click += TableStyleMenuItem_Click;
                contextMenu.Items.Add(item);
            }

            Rectangle bounds = _sbPanelTableStyle.Bounds;
            Point location = e.Location;
            contextMenu.Show(_statusBar, bounds.X + location.X, bounds.Y + location.Y);
        }
    }

    private void bToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ExecuteQuerySingleRow();
    }

    private void textBox_SelectionChanged(object sender, EventArgs e)
    {
        RichTextBox richTextBox = (RichTextBox)sender;
        int charIndex = richTextBox.SelectionStart;
        int line = richTextBox.GetLineFromCharIndex(charIndex) + 1;
        int lineIndex = QueryTextBox.GetLineIndex(richTextBox, -1);
        int col = charIndex - lineIndex + 1;
        _sbPanelCaretPosition.Text = "Ln " + line + " Col " + col;
    }

    private void mnuDescribeParameters_Click(object sender, EventArgs e)
    {
        try
        {
            Cursor = Cursors.WaitCursor;

            if (Connection.Connection is OleDbConnection oleDbConnection && string.IsNullOrEmpty(Query))
            {
                DataSet dataSet = new DataSet();
                AddTable(oleDbConnection, dataSet, OleDbSchemaGuid.Provider_Types, "Provider Types");
                AddTable(oleDbConnection, dataSet, OleDbSchemaGuid.DbInfoLiterals, "DbInfoLiterals");

                ConnectionClass adoDbConnection = new ConnectionClass();
                adoDbConnection.Open(_connectionInfo.ConnectionStringAndCredential.ConnectionString, null, null, 0);
                Recordset rs = adoDbConnection.OpenSchema(SchemaEnum.adSchemaDBInfoKeywords, Type.Missing, Type.Missing);
                DataTable dataTable = OleDbHelper.Convert(rs);
                adoDbConnection.Close();
                dataSet.Tables.Add(dataTable);

                AddTable(oleDbConnection, dataSet, OleDbSchemaGuid.Sql_Languages, "Sql Languages");
                ShowDataSet(dataSet);
            }
            else
            {
                _sqlStatement = new SqlParser(Query);
                _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

                if (_command != null)
                {
                    AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, _command.ToLogString()));
                    DataTable dataTable = Provider.GetParameterTable(_command.Parameters);

                    if (dataTable != null)
                    {
                        dataTable.TableName = "Parameters";

                        foreach (DataRow row in dataTable.Rows)
                        {
                            object value = row["Value"];
                            Type type = value.GetType();
                            TypeCode typeCode = Type.GetTypeCode(type);

                            switch (typeCode)
                            {
                                case TypeCode.DateTime:
                                    DateTime dateTime = (DateTime)value;
                                    long ticks = dateTime.Ticks;

                                    if (ticks % StopwatchConstants.TicksPerDay == 0)
                                        row["Value"] = dateTime.ToString("yyyy-MM-dd");
                                    else
                                        row["Value"] = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    break;
                            }
                        }

                        DataSet dataSet = new DataSet();
                        dataSet.Tables.Add(dataTable);
                        ShowDataSet(dataSet);
                    }
                }
            }

            Cursor = Cursors.Default;
        }
        catch (Exception ex)
        {
            Cursor = Cursors.Default;
            ShowMessage(ex);
        }
    }

    private void mnuCloseTabPage_Click(object sender, EventArgs e)
    {
        TabPage? tabPage = _tabControl.SelectedTab;

        if (tabPage != null && tabPage != _messagesTabPage && tabPage != _resultSetsTabPage)
            CloseResultSetTabPage(tabPage);
    }

    private void mnuCloseAllTabPages_Click(object sender, EventArgs e)
    {
        CloseResultSetTabPages();

        _tabControl.SelectedTab = _messagesTabPage;
        _messagesTextBox.Clear();
        SetStatusbarPanelText(null);

        if (_dataAdapter == null)
        {
            _sbPanelRows.Text = null;
            _sbPanelTimer.Text = null;
        }

        this.Invoke(() => FocusControl(QueryTextBox));
    }

    private void mnuCancel_Click(object sender, EventArgs e) => CancelCommandQuery();

    private void tvObjectBrowser_DoubleClick(object sender, EventArgs e)
    {
        TreeNode? selectedNode = _tvObjectExplorer.SelectedNode;
        if (selectedNode != null)
        {
            ITreeNode? treeNode = (ITreeNode)selectedNode.Tag;

            try
            {
                Cursor = Cursors.WaitCursor;
                string query = treeNode.Query;
                if (query != null)
                {
                    string text0 = QueryTextBox.Text;
                    string append = null;
                    int selectionStart = QueryTextBox.RichTextBox.TextLength;

                    if (!string.IsNullOrEmpty(text0))
                    {
                        append = Environment.NewLine + Environment.NewLine;
                        selectionStart += 2;
                    }

                    append += query;

                    QueryTextBox.RichTextBox.AppendText(append);
                    QueryTextBox.RichTextBox.SelectionStart = selectionStart;
                    QueryTextBox.RichTextBox.SelectionLength = query.Length;

                    QueryTextBox.Focus();
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }

    private void TableStyleMenuItem_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem item = (ToolStripMenuItem)sender;
        ResultWriterType tableStyle = (ResultWriterType)item.Tag;
        SetResultWriterType(tableStyle);
    }

    private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e) => ExecuteQuery();
    private void aToolStripMenuItem_Click(object sender, EventArgs e) => ExecuteQuery();
    private void cancelExecutingQueryButton_Click(object sender, EventArgs e) => CancelCommandQuery();
    private void toolStripMenuItem1_Click(object sender, EventArgs e) => ExecuteQuery();
    private void editRowsToolStripMenuItem_Click(object sender, EventArgs e) => EditRows(Query);

    private void parseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IDbCommandExecutor executor = Connection.Connection.CreateCommandExecutor();
        bool on = false;
        try
        {
            executor.ExecuteNonQuery(new CreateCommandRequest("SET PARSEONLY ON"));
            on = true;
            string query = Query;
            bool succeeded;

            try
            {
                executor.ExecuteNonQuery(new CreateCommandRequest(query));
                succeeded = _infoMessages.IsEmpty;
            }
            catch (Exception exception)
            {
                succeeded = false;
                List<InfoMessage> infoMessages = Provider.ToInfoMessages(exception);
                AddInfoMessages(infoMessages);
            }

            if (succeeded)
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Command(s) completed successfully."));
        }
        catch (Exception exception)
        {
            List<InfoMessage> infoMessages = Provider.ToInfoMessages(exception);
            AddInfoMessages(infoMessages);
        }

        if (on)
            executor.ExecuteNonQuery(new CreateCommandRequest("SET PARSEONLY OFF"));
    }

    private void createCCommandQueryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        const string text = @"/* Query Configuration
{
  ""Using"": ""using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;"",
  ""Namespace"": ""Company.Product.CommandOrQueryName"",
  ""Name"": ""CommandOrQueryName"",
  ""Results"": [
    ""Result1Item(s)"",
    ""Result2Item(s)"",
  ]
}
*/
declare @int int = 0
declare @date date = getdate()
-- CommandText
select
    @int as [Int],
    @date as [Date]
	
select
    @int as [Int],
    @date as [Date]";

        AppendQueryText(text);
    }

    private void undoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //var canUndo = _queryTextBox.RichTextBox.CanUndo;
        //if (canUndo)
        //{
        //    var actionName = _queryTextBox.RichTextBox.UndoActionName;
        //    Trace.WriteLine($"UndoActionName:{actionName}");
        //    _queryTextBox.RichTextBox.Undo();
        //    _queryTextBox.RichTextBox.ClearUndo();
        //}

        _queryTextBox.Undo();
    }
}