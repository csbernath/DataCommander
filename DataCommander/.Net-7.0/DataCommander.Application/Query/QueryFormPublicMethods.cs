using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using DataCommander.Application.ResultWriter;
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

    public void AppendQueryText(string text)
    {
        QueryTextBox.RichTextBox.AppendText(text);
    }

    public void ShowXml(string tabPageName, string xml)
    {
        var htmlTextBox = new HtmlTextBox();
        htmlTextBox.Dock = DockStyle.Fill;

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

    public void SetStatusbarPanelText(string text)
    {
        var color = _colorTheme != null ? _colorTheme.ForeColor : SystemColors.ControlText;
        SetStatusbarPanelText(text, color);
    }
}