using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using DataCommander.Application.ResultWriter;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Application.Query;

public sealed partial class QueryForm
{
    public void AddDataTable(DataTable dataTable, ResultWriterType tableStyle)
    {
        switch (tableStyle)
        {
            case ResultWriterType.Text:
                ShowDataTableText(dataTable);
                break;

            case ResultWriterType.DataGrid:
                ShowDataTableDataGrid(dataTable);
                break;

            //case ResultWriterType.Html:
            //    ShowDataViewHtml(dataTable.DefaultView);
            //    break;

            case ResultWriterType.Rtf:
                ShowDataTableRtf(dataTable);
                break;

            case ResultWriterType.ListView:
                ShowDataTableListView(dataTable);
                break;

            case ResultWriterType.Excel:
                ShowDataTableExcel(dataTable);
                break;
        }
    }

    public void AppendQueryText(string text) => QueryTextBox.RichTextBox.AppendText(text);

    public void ShowXml(string tabPageName, string xml)
    {
        var htmlTextBox = new HtmlTextBox
        {
            Dock = DockStyle.Fill
        };

        var tabPage = new TabPage(tabPageName);
        _tabControl.TabPages.Add(tabPage);
        tabPage.Controls.Add(htmlTextBox);

        htmlTextBox.Xml = xml;
    }

    public void AddInfoMessage(InfoMessage infoMessage)
    {
        WriteInfoMessageToLog(infoMessage);

        if (infoMessage.Severity == InfoMessageSeverity.Error)
            _errorCount++;

        _infoMessages.Enqueue(infoMessage);
        _enqueueEvent.Set();
    }

    public void Save()
    {
        if (_fileName != null)
            Save(_fileName);
        else
            ShowSaveFileDialog();
    }

    public void LoadFile(string path)
    {
        string text;

        using (var reader = new StreamReader(path, Encoding.Default, true))
        {
            Log.Write(LogLevel.Trace, "reader.CurrentEncoding.EncodingName: {0}", reader.CurrentEncoding.EncodingName);
            text = reader.ReadToEnd();
        }

        QueryTextBox.Text = text;
        _fileName = path;
        SetText();
        SetStatusbarPanelText($"File {_fileName} loaded successfully.");
    }

    public void SetStatusbarPanelText(string? text)
    {
        var color = _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText;
        SetStatusbarPanelText(text, color);
    }

    [AllowNull]
    public override Font Font
    {
        set
        {
            _font = value!;
            _queryTextBox.Font = value;
            var size1 = TextRenderer.MeasureText("1", value);
            var size2 = TextRenderer.MeasureText("12", value);
            var width = _queryTextBox.TabSize * (size2.Width - size1.Width);
            var tabs = new int[12];

            for (var i = 0; i < tabs.Length; i++)
                tabs[i] = (i + 1) * width;

            //_queryTextBox.EnableChangeEvent(false);
            _queryTextBox.RichTextBox.Font = value;
            _queryTextBox.RichTextBox.SelectionTabs = tabs;
            //_queryTextBox.EnableChangeEvent(true);

            _messagesTextBox.Font = value;
            _messagesTextBox.SelectionTabs = tabs;
        }
    }

    public void ShowDataSet(DataSet? dataSet)
    {
        using var log = LogFactory.Instance.GetCurrentMethodLog();
        if (dataSet != null && dataSet.Tables.Count > 0)
        {
            GetTableSchemaResult getTableSchemaResult = null;
            string? text;
            if (_openTableMode)
            {
                var tableName = _sqlStatement.FindTableName();
                text = tableName;
                dataSet.Tables[0].TableName = tableName;
                getTableSchemaResult = Provider.GetTableSchema(Connection!.Connection!, tableName);
            }
            else
            {
                ResultSetCount++;
                text = $"Set {ResultSetCount}";
            }

            var resultSetTabPage = new TabPage(text);
            GarbageMonitor.Default.Add("resultSetTabPage", resultSetTabPage);
            resultSetTabPage.ToolTipText = null; // TODO
            _resultSetsTabControl.TabPages.Add(resultSetTabPage);
            _resultSetsTabControl.SelectedTab = resultSetTabPage;
            if (dataSet.Tables.Count > 1)
            {
                var tabControl = new TabControl { Dock = DockStyle.Fill };
                tabControl.MouseUp += DataTableTabControl_MouseUp;

                var index = 0;
                foreach (DataTable dataTable in dataSet.Tables)
                {
                    var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
                    var control = QueryFormStaticMethods.CreateControlFromDataTable(this, commandBuilder, dataTable, getTableSchemaResult, TableStyle,
                        !_openTableMode, _colorTheme);
                    control.Dock = DockStyle.Fill;
                    var rowCount = StringExtensions.SingularOrPlural(dataTable.Rows.Count, "row", "rows");
                    text = $"{dataTable.TableName} ({rowCount})";
                    var tabPage = new TabPage(text)
                    {
                        ToolTipText = null // TODO
                    };
                    tabPage.Controls.Add(control);
                    tabControl.TabPages.Add(tabPage);
                    index++;
                }

                resultSetTabPage.Controls.Add(tabControl);
            }
            else
            {
                var commandBuilder = Provider.DbProviderFactory.CreateCommandBuilder();
                var control = QueryFormStaticMethods.CreateControlFromDataTable(this, commandBuilder, dataSet.Tables[0], getTableSchemaResult, TableStyle,
                    !_openTableMode, _colorTheme);
                control.Dock = DockStyle.Fill;
                resultSetTabPage.Controls.Add(control);
            }
        }
    }

    public void ShowMessage(Exception exception)
    {
        var infoMessages = Provider.ToInfoMessages(exception);
        AddInfoMessages(infoMessages);

        _tabControl.SelectedTab = _messagesTabPage;

        SetStatusbarPanelText("Query batch completed with errors.", _colorTheme != null ? _colorTheme.ProviderKeyWordColor : Color.Red);
        _sbPanelRows.Text = null;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                //var now = LocalTime2.Default.Now;
                //foreach (IComponent component in components.Components)
                //    GarbageMonitor.Default.SetDisposeTime(component, now);

                components.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    public void EditRows(string query)
    {
        var succeeded = EnsureConnectionIsOpen();
        if (succeeded)
        {
            try
            {
                Log.Write(LogLevel.Trace, "Query:\r\n{0}", query);
                _sqlStatement = new SqlParser(query);
                _commandType = CommandType.Text;
                _openTableMode = true;
                _command = _sqlStatement.CreateCommand(Provider, Connection, _commandType, _commandTimeout);
                AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, "Executing query..."));
                _stopwatch.Start();
                _timer.Start();
                const int maxRecords = int.MaxValue;
                _dataSetResultWriter = new DataSetResultWriter(AddInfoMessage, _showSchemaTable);
                var resultWriter = _dataSetResultWriter;
                _dataAdapter = new AsyncDataAdapter(Provider, maxRecords, _rowBlockSize, resultWriter, EndFillInvoker, WriteEndInvoker);
                _dataAdapter.Start(new AsyncDataAdapterCommand(null, 0, _command, null, null, null).ItemToArray());
            }
            catch (Exception ex)
            {
                EndFill(_dataAdapter, ex);
            }
        }
    }

    public void ShowText(string text)
    {
        var mainForm = DataCommanderApplication.Instance.MainForm!;
        mainForm.Cursor = Cursors.WaitCursor;

        try
        {
            var selectionStart = _queryTextBox.RichTextBox.TextLength;
            var append = text;
            _queryTextBox.RichTextBox.AppendText(append);
            _queryTextBox.RichTextBox.SelectionStart = selectionStart;
            _queryTextBox.RichTextBox.SelectionLength = append.Length;

            _queryTextBox.Focus();
        }
        catch (Exception e)
        {
            ShowMessage(e);
        }
        finally
        {
            mainForm.Cursor = Cursors.Default;
        }
    }

    public void SetClipboardText(string text) => Clipboard.SetText(text);
}