using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using DataCommander.Application.ResultWriter;
using Foundation.Core;
using Foundation.Data;
using Foundation.Linq;
using Foundation.Log;
using Foundation.Text;
using Foundation.Threading;

namespace DataCommander.Application.Query;

public sealed partial class QueryForm
{
    private static void CloseResultTabPage(TabPage tabPage)
    {
        foreach (Control control in tabPage.Controls)
            control.Dispose();

        tabPage.Controls.Clear();
    }

    private void CloseResultSetTabPage(TabPage tabPage)
    {
        _resultSetsTabControl.TabPages.Remove(tabPage);
        Control control = tabPage.Controls[0];
        if (control is TabControl tabControl)
        {
            System.Collections.Generic.List<TabPage> tabPages = tabControl.TabPages.Cast<TabPage>().ToList();
            foreach (TabPage? subTabPage in tabPages)
            {
                tabControl.TabPages.Remove(subTabPage);
                CloseResultTabPage(subTabPage);
            }
        }
        else
            CloseResultTabPage(tabPage);
    }

    private bool SaveTextOnFormClosing()
    {
        bool cancel = false;
        int length = QueryTextBox.Text.Length;
        if (length > 0)
        {
            string text = $"The text in {Text} has been changed.\r\nDo you want to save the changes?";
            string caption = DataCommanderApplication.Instance.Name;
            DialogResult result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            switch (result)
            {
                case DialogResult.Yes:
                    if (_fileName != null)
                        Save(_fileName);
                    else
                        ShowSaveFileDialog();

                    break;

                case DialogResult.No:
                    break;

                case DialogResult.Cancel:
                    cancel = true;
                    break;
            }
        }

        return cancel;
    }

    private bool CancelQueryOnFormClosing()
    {
        bool cancel = false;
        if (_dataAdapter != null)
        {
            string text = "Are you sure you wish to cancel this query?";
            string caption = DataCommanderApplication.Instance.Name;
            DialogResult result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
                CancelCommandQuery();
                _timer.Enabled = false;
            }
            else
                cancel = true;
        }

        return cancel;
    }

    private bool AskUserToCommitTransactions()
    {
        bool cancel = false;
        string text = "There are uncommitted transaction(s). Do you wish to commit these transaction(s) before closing the window?";
        string caption = DataCommanderApplication.Instance.Name;
        DialogResult result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        switch (result)
        {
            case DialogResult.Yes:
            case DialogResult.Cancel:
                cancel = true;
                break;

            case DialogResult.No:
                break;
        }

        return cancel;
    }

    private bool CommitTransactionOnFormClosing()
    {
        bool cancel = false;
        if (Connection is { State: ConnectionState.Open })
        {
            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;
                CancelableOperationForm cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, TimeSpan.FromSeconds(1),
                    "Getting transaction count...", string.Empty, _colorTheme);
                int transactionCount = cancelableOperationForm.Execute(new Task<int>(() => Connection.GetTransactionCountAsync(cancellationToken).Result));
                bool hasTransactions = transactionCount > 0;
                if (hasTransactions)
                    cancel = AskUserToCommitTransactions();
            }
            catch (Exception exception)
            {
                string text = exception.ToString();
                string caption = "Getting transaction count failed. Close window?";
                DialogResult dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.No)
                    cancel = true;
            }
        }

        return cancel;
    }

    private void CloseConnectionOnFormClosing()
    {
        _cancellationTokenSource.Cancel();

        if (Connection != null)
        {
            string dataSource = Connection.DataSource;
            _parentStatusBar.Items[0].Text = $"Closing connection to data source {dataSource}'....";
            Connection.Close();
            _parentStatusBar.Items[0].Text = $"Connection to data source {dataSource} closed.";
            Connection.Connection.Dispose();
            Connection = null;
        }

        if (_toolStrip != null)
        {
            _toolStrip.Dispose();
            _toolStrip = null;
        }
    }

    private void SetResultWriterType(ResultWriterType tableStyle)
    {
        TableStyle = tableStyle;
        _sbPanelTableStyle.Text = tableStyle.ToString();
    }

    private void AddTable(OleDbConnection oleDbConnection, DataSet dataSet, Guid guid, string name)
    {
        try
        {
            DataTable? dataTable = oleDbConnection.GetOleDbSchemaTable(guid, null);
            dataTable.TableName = name;
            dataSet.Tables.Add(dataTable);
        }
        catch (Exception e)
        {
            _messagesTextBox.Text += e.ToString();
        }
    }

    private void CloseResultSetTabPages()
    {
        TabPage[] tabPages = _resultSetsTabControl.TabPages.Cast<TabPage>().ToArray();

        foreach (TabPage? tabPage in tabPages)
            CloseResultSetTabPage(tabPage);

        ResultSetCount = 0;
    }

    private void CancelCommandQuery()
    {
        Log.Trace(ThreadMonitor.ToStringTableString());
        const string message = "Canceling command...";
        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, message));
        SetStatusbarPanelText("Cancel Executing Command/Query...");
        _cancel = true;
        SetGui(CommandState.None);
        _dataAdapter.Cancel();
    }

    private void WriteRows(long rowCount, int scale)
    {
        long ticks = _stopwatch.ElapsedTicks;
        _sbPanelTimer.Text = StopwatchTimeSpan.ToString(ticks, scale);
        string text = rowCount + " rows.";
        if (rowCount > 0)
        {
            double seconds = (double)ticks / Stopwatch.Frequency;
            text += " (" + Math.Round(rowCount / seconds, 0) + " rows/sec)";
        }

        _sbPanelRows.Text = text;
    }

    private void ShowTimer()
    {
        if (_dataAdapter != null)
        {
            long rowCount = _dataAdapter.RowCount;
            WriteRows(rowCount, 0);
        }
    }

    private ContextMenuStrip? GetContextMenu(ITreeNode treeNode)
    {
        ContextMenu? contextMenu = treeNode.GetContextMenu();
        ContextMenuStrip? contextMenuStrip = contextMenu != null
            ? ToContextMenuStrip(contextMenu)
            : null;
        return contextMenuStrip;
    }

    private ContextMenuStrip ToContextMenuStrip(ContextMenu contextMenu)
    {
        ToolStripItem[] menuItems = contextMenu.MenuItems
            .Select(ToToolStripMenuItem)
            .ToArray();

        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.AddRange(menuItems);

        return contextMenuStrip;
    }

    private ToolStripMenuItem ToToolStripMenuItem(MenuItem source)
    {
        ToolStripMenuItem item = new ToolStripMenuItem(source.Text, null, (sender, args) =>
        {
            try
            {
                source.OnClick(this, args);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        });
        ToolStripItem[] dropdownItems = source.DropDownItems
            .Select(ToToolStripMenuItem)
            .Cast<ToolStripItem>()
            .ToArray();
        item.DropDownItems.AddRange(dropdownItems);
        return item;
    }

    private static TreeNode FindTreeNode(TreeNode parent, IStringMatcher matcher)
    {
        TreeNode found = null;

        if (matcher.IsMatch(parent.Text))
            found = parent;
        else
        {
            foreach (TreeNode child in parent.Nodes)
            {
                found = FindTreeNode(child, matcher);

                if (found != null)
                    break;
            }
        }

        return found;
    }

    private void FindText(string text)
    {
        bool found = false;
        Control? control = ActiveControl;

        try
        {
            Cursor = Cursors.WaitCursor;
            SetStatusbarPanelText($"Finding {text}...");
            RichTextBoxFinds options = _findTextForm.RichTextBoxFinds;
            StringComparison comparison = options switch
            {
                RichTextBoxFinds.None => StringComparison.InvariantCultureIgnoreCase,
                RichTextBoxFinds.MatchCase => StringComparison.InvariantCulture,
                RichTextBoxFinds.WholeWord => throw new NotImplementedException(),// TODO
                _ => throw new NotImplementedException(),
            };
            IStringMatcher matcher = new StringMatcher(text, comparison);

            if (control is TreeView treeView)
            {
                TreeNode? treeNode2 = treeView.SelectedNode.FirstNode;

                if (treeNode2 == null || treeNode2.Tag == null)
                    treeNode2 = treeView.SelectedNode.NextNode;

                TreeNode? treeNode = treeNode2;

                while (treeNode != null)
                {
                    treeNode2 = FindTreeNode(treeNode, matcher);

                    if (treeNode2 != null)
                    {
                        treeView.SelectedNode = treeNode2;
                        found = true;
                        break;
                    }
                    else
                        treeNode = treeNode.NextNode;
                }
            }
            else
            {
                DataTableEditor? dataTableEditor = control as DataTableEditor;

                if (dataTableEditor == null)
                    dataTableEditor = control.Parent as DataTableEditor;

                if (dataTableEditor != null)
                {
                    DataTable dataTable = dataTableEditor.DataTable;

                    if (dataTable != null)
                    {
                        if (text.StartsWith("RowFilter="))
                        {
                            string rowFilter = text[5..];
                            DataView dataView = dataTable.DefaultView;
                            dataView.RowFilter = rowFilter;
                            int count = dataView.Count;
                            found = count > 0;
                            SetStatusbarPanelText($"{count} rows found. RowFilter: {rowFilter}");
                        }
                        else if (text.StartsWith("Sort="))
                        {
                            string sort = text[5..];
                            DataView dataView = dataTable.DefaultView;
                            dataView.Sort = sort;
                            SetStatusbarPanelText($"Rows sorted by {sort}.");
                        }
                        else
                        {
                            DataGridView dataGrid = dataTableEditor.DataGrid;
                            DataGridViewCell? cell = dataGrid.CurrentCell;
                            int rowIndex = cell.RowIndex;
                            int columnIndex = cell.ColumnIndex;
                            found = QueryFormStaticMethods.FindText(dataTable.DefaultView, matcher, ref rowIndex, ref columnIndex);

                            if (found)
                            {
                                dataGrid.CurrentCell = dataGrid[columnIndex, rowIndex];
                            }
                        }
                    }
                    else
                        found = false;
                }
                else
                {
                    if (control is not RichTextBox richTextBox)
                    {
                        richTextBox = QueryTextBox.RichTextBox;
                    }

                    int start = richTextBox.SelectionStart + richTextBox.SelectionLength;
                    int location = richTextBox.Find(text, start, options);
                    found = location >= 0;
                }
            }
        }
        finally
        {
            SetStatusbarPanelText(null);
            Cursor = Cursors.Default;
        }

        if (!found)
        {
            string message = $"The specified text was not found.\r\n\r\nText: {text}\r\nControl: {control.Name}";
            MessageBox.Show(this, message, DataCommanderApplication.Instance.Name);
        }
    }

    private void Save(string fileName)
    {
        Cursor = Cursors.WaitCursor;

        try
        {
            SetStatusbarPanelText($"Saving file {fileName}...");
            const RichTextBoxStreamType type = RichTextBoxStreamType.UnicodePlainText;
            Encoding encoding = Encoding.Unicode;

            using (FileStream stream = File.Create(fileName))
            {
                byte[] preamble = encoding.GetPreamble();
                stream.Write(preamble, 0, preamble.Length);
                QueryTextBox.RichTextBox.SaveFile(stream, type);
            }

            _fileName = fileName;
            SetText();
            SetStatusbarPanelText($"File {fileName} saved successfully.");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void ShowSaveFileDialog()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Save Query",
            Filter = "Query Files (*.sql)|*.sql",
            AddExtension = true,
            OverwritePrompt = true,
            DefaultExt = "sql"
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string fileName = saveFileDialog.FileName;
            Save(fileName);
        }
    }

    private GetCompletionResult GetCompletion()
    {
        RichTextBox textBox = QueryTextBox.RichTextBox;
        string text = textBox.Text;
        int position = textBox.SelectionStart;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TimeSpan showDialogDelay = TimeSpan.FromSeconds(1);
        const string formText = "Getting completion result...";
        const string textBoxText = "Please wait...";
        CancelableOperationForm cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, showDialogDelay, formText, textBoxText, _colorTheme);
        long startTimestamp = Stopwatch.GetTimestamp();
        GetCompletionResult? response = cancelableOperationForm.Execute(new Task<GetCompletionResult?>(() =>
            Provider.GetCompletion(Connection, _transaction, text, position, cancellationTokenSource.Token).Result));
        long elapsed = Stopwatch.GetTimestamp() - startTimestamp;
        string from = response.FromCache ? "cache" : "data source";
        int count = response.Items != null ? response.Items.Count : 0;
        SetStatusbarPanelText($"GetCompletion returned {count} items from {from} in {StopwatchTimeSpan.ToString(elapsed, 3)} seconds.");
        return response;
    }

    private async void ExecuteReader(CommandBehavior commandBehavior)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

            if (_command != null)
            {
                IDataReader dataReader = null;

                try
                {
                    while (true)
                    {
                        try
                        {
                            dataReader = _command.ExecuteReader(commandBehavior);
                            break;
                        }
                        catch
                        {
                            if (Connection.State != ConnectionState.Open)
                            {
                                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Opening connection..."));
                                await Connection.OpenAsync(CancellationToken.None);
                                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Connection opened successfully."));
                            }
                            else
                                throw;
                        }
                    }

                    DataSet dataSet = null;
                    int i = 1;

                    do
                    {
                        DataTable dataTable = Provider.GetSchemaTable(dataReader);

                        if (dataTable != null)
                        {
                            if (dataSet == null)
                                dataSet = new DataSet();

                            dataTable.TableName = "SchemaTable" + i;
                            dataSet.Tables.Add(dataTable);
                            i++;
                        }
                    } while (dataReader.NextResult());

                    ShowDataSet(dataSet);
                    _tabControl.SelectedTab = _resultSetsTabPage;
                }
                finally
                {
                    if (dataReader != null)
                        dataReader.Close();
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

    private void ExecuteQuerySingleRow()
    {
        try
        {
            _sqlStatement = new SqlParser(Query);
            _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
            DataSet dataSet = new DataSet();
            using (DbDataReader dataReader = _command.ExecuteReader())
            {
                int tableIndex = 0;

                while (true)
                {
                    DataTable schemaTable = Provider.GetSchemaTable(dataReader);
                    IDataReaderHelper dataReaderHelper = Provider.CreateDataReaderHelper(dataReader);
                    int rowIndex = 0;

                    while (dataReader.Read())
                    {
                        object[] values = new object[dataReader.FieldCount];
                        dataReaderHelper.GetValues(values);

                        DataTable dataTable = new DataTable($"Table[{tableIndex}].Rows[{rowIndex}]");
                        dataTable.Columns.Add(" ", typeof(int));
                        dataTable.Columns.Add("Name", typeof(string));
                        dataTable.Columns.Add("Value");
                        int count = schemaTable.Rows.Count;

                        for (int i = 0; i < count; ++i)
                        {
                            DataRow schemaRow = schemaTable.Rows[i];
                            string? columnName = schemaRow["Name"].ToString();

                            DataRow dataRow = dataTable.NewRow();
                            dataRow[0] = i + 1;
                            dataRow[1] = columnName;
                            dataRow[2] = values[i];
                            dataTable.Rows.Add(dataRow);
                        }

                        dataSet.Tables.Add(dataTable);

                        if (rowIndex == 100)
                            break;

                        ++rowIndex;
                    }

                    if (!dataReader.NextResult())
                        break;

                    ++tableIndex;
                }
            }

            ShowDataSet(dataSet);
            _tabControl.SelectedTab = _resultSetsTabPage;
        }
        catch (Exception ex)
        {
            ShowMessage(ex);
        }
    }

    private void SetTransaction(DbTransaction transaction)
    {
        if (_transaction == null && transaction != null)
        {
            _transaction = transaction;
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Transaction created successfully."));
            _tabControl.SelectedTab = _messagesTabPage;
            _beginTransactionToolStripMenuItem.Enabled = false;
            _commitTransactionToolStripMenuItem.Enabled = true;
            _rollbackTransactionToolStripMenuItem.Enabled = true;
        }
    }

    private void InvokeSetTransaction(DbTransaction transaction) => Invoke(() => SetTransaction(transaction));

    internal void ScriptQueryAsCreateTable()
    {
        SqlParser sqlStatement = new SqlParser(Query);
        DbCommand command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);

        Form[] forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
        int index = Array.IndexOf(forms, this);
        IProvider destinationProvider;

        if (index < forms.Length - 1)
        {
            QueryForm nextQueryForm = (QueryForm)forms[index + 1];
            destinationProvider = nextQueryForm.Provider;
        }
        else
            destinationProvider = Provider;

        DataTable schemaTable;
        string[] dataTypeNames;

        using (DbDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
        {
            schemaTable = dataReader.GetSchemaTable();
            dataTypeNames = new string[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; i++)
                dataTypeNames[i] = dataReader.GetDataTypeName(i);
        }

        string? tableName = command.CommandType == CommandType.StoredProcedure ? command.CommandText : sqlStatement.FindTableName();
        StringBuilder createTable = new StringBuilder();
        createTable.AppendFormat("create table [{0}]\r\n(\r\n", tableName);
        StringTable stringTable = new StringTable(3);
        int last = schemaTable.Rows.Count - 1;

        for (int i = 0; i <= last; i++)
        {
            DataRow dataRow = schemaTable.Rows[i];
            FoundationDbColumn schemaRow = FoundationDbColumnFactory.Create(dataRow);
            StringTableRow row = stringTable.NewRow();
            string typeName = destinationProvider.GetColumnTypeName(Provider, dataRow, dataTypeNames[i]);
            row[1] = schemaRow.ColumnName;
            row[2] = typeName;
            bool? allowDbNull = schemaRow.AllowDbNull;

            if (allowDbNull == false)
                row[2] += " not null";

            if (i < last)
                row[2] += ',';

            stringTable.Rows.Add(row);
        }

        createTable.Append(stringTable.ToString(4));
        createTable.Append(')');
        string commandText = createTable.ToString();

        AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "\r\n" + commandText));
    }

    internal void CopyTable()
    {
        Form[] forms = DataCommanderApplication.Instance.MainForm.MdiChildren;
        int index = Array.IndexOf(forms, this);
        if (index < forms.Length - 1)
        {
            QueryForm nextQueryForm = (QueryForm)forms[index + 1];
            IProvider destinationProvider = nextQueryForm.Provider;
            ConnectionBase? destinationConnection = nextQueryForm.Connection;
            SqlParser sqlStatement = new SqlParser(Query);
            _command = sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
            string? tableName = _command.CommandType == CommandType.StoredProcedure ? _command.CommandText : sqlStatement.FindTableName();

            if (tableName[0] == '[' && destinationProvider.Identifier == "System.Data.OracleClient")
                tableName = tableName[1..^1];

            IResultWriter resultWriter = new CopyResultWriter(AddInfoMessage, destinationProvider, destinationConnection, tableName,
                nextQueryForm.InvokeSetTransaction, CancellationToken.None);
            int maxRecords = int.MaxValue;
            int rowBlockSize = 10000;
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, "Copying table..."));
            SetStatusbarPanelText("Copying table...");
            SetGui(CommandState.Cancel);
            _errorCount = 0;
            _stopwatch.Start();
            _timer.Start();
            _dataAdapter = new AsyncDataAdapter(Provider, maxRecords, rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
            _dataAdapter.Start(new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray());
        }
        else
            AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Please open a destination connection."));
    }

    private void redoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //var canRedo = _queryTextBox.RichTextBox.CanRedo;
        //if (canRedo)
        //{
        //    var actionName = _queryTextBox.RichTextBox.RedoActionName;
        //    Trace.WriteLine($"RedoActionName:{actionName}");
        //    _queryTextBox.RichTextBox.Redo();
        //}

        _queryTextBox.Redo();
    }
}