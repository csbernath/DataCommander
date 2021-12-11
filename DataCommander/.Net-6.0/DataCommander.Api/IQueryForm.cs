using System;
using System.Data;
using System.Drawing;

namespace DataCommander.Api;

public interface IQueryForm
{
    void ShowMessage(Exception exception);
    void ShowDataSet(DataSet dataSet);
    void SetStatusbarPanelText(string text, Color color);
    ColorTheme ColorTheme { get; }
    void ShowText(string text);
    void ClipboardSetText(string text);
    void EditRows(string query);
}