using System;
using System.Data;
using System.Threading;

namespace DataCommander.Api;

public interface IQueryForm
{
    void ShowMessage(Exception exception);
    void ShowDataSet(DataSet dataSet);
    void SetStatusbarPanelText(string text);
    void ShowText(string text);
    void SetClipboardText(string text);
    void EditRows(string query);
    ICancelableOperationForm CreateCancelableOperationForm(CancellationTokenSource cancellationTokenSource, string formText, string textBoxText);
}