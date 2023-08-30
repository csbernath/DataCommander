using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api.Query;
using Foundation.Linq;

namespace DataCommander.Application.Query;

public sealed partial class QueryForm
{
    private void CloseResultTabPage(TabPage tabPage)
    {
        foreach (Control control in tabPage.Controls)
            control.Dispose();

        tabPage.Controls.Clear();
    }

    private void CloseResultSetTabPage(TabPage tabPage)
    {
        _resultSetsTabControl.TabPages.Remove(tabPage);
        var control = tabPage.Controls[0];
        if (control is TabControl tabControl)
        {
            var tabPages = tabControl.TabPages.Cast<TabPage>().ToList();
            foreach (var subTabPage in tabPages)
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
        var cancel = false;
        var length = QueryTextBox.Text.Length;
        if (length > 0)
        {
            var text = $"The text in {Text} has been changed.\r\nDo you want to save the changes?";
            var caption = DataCommanderApplication.Instance.Name;
            var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        var cancel = false;
        if (_dataAdapter != null)
        {
            var text = "Are you sure you wish to cancel this query?";
            var caption = DataCommanderApplication.Instance.Name;
            var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        var cancel = false;
        var text = "There are uncommitted transaction(s). Do you wish to commit these transaction(s) before closing the window?";
        var caption = DataCommanderApplication.Instance.Name;
        var result = MessageBox.Show(this, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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
        var cancel = false;
        if (Connection is { State: ConnectionState.Open })
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                var getTransactionCountTask = new Task<int>(() => Connection.GetTransactionCountAsync(cancellationToken).Result);
                var cancelableOperationForm = new CancelableOperationForm(this, cancellationTokenSource, "Getting transaction count...",
                    string.Empty, _colorTheme);
                cancelableOperationForm.Start(getTransactionCountTask, TimeSpan.FromSeconds(1));
                var transactionCount = getTransactionCountTask.Result;
                var hasTransactions = transactionCount > 0;
                if (hasTransactions)
                    cancel = AskUserToCommitTransactions();
            }
            catch (Exception exception)
            {
                var text = exception.ToString();
                var caption = "Getting transaction count failed. Close window?";
                var dialogResult = MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
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
            var dataSource = Connection.DataSource;
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
}