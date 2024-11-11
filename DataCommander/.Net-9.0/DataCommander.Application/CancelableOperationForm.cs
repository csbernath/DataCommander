using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;
using Foundation.Core;
using Foundation.Windows.Forms;

namespace DataCommander.Application;

public sealed partial class CancelableOperationForm : Form, ICancelableOperationForm
{
    private readonly Control _owner;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TimeSpan _showDialogDelay;
    private System.Threading.Timer? _elapsedTimeTimer;
    private long _startTimestamp;

    public CancelableOperationForm(
        Control owner,
        CancellationTokenSource cancellationTokenSource,
        TimeSpan showDialogDelay,
        string formText,
        string textBoxText,
        ColorTheme? colorTheme)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner;
        _cancellationTokenSource = cancellationTokenSource;
        _showDialogDelay = showDialogDelay;
        InitializeComponent();
        colorTheme?.Apply(this);
        Text = formText;
        textBox.AppendText(textBoxText);
        ActiveControl = cancelButton;
    }
    
    public T Execute<T>(Task<T> cancelableOperation)
    {
        Execute((Task)cancelableOperation);
        return cancelableOperation.Result;
    }

    public void Execute(Task cancelableOperation)
    {
        StartAndWaitBeforeShowDialog(cancelableOperation);
        ShowDialogIfNotCompleted(cancelableOperation);
    }

    private void StartAndWaitBeforeShowDialog(Task cancelableOperation)
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        var cancellationToken = _cancellationTokenSource.Token;
        cancelableOperation.ContinueWith(_ =>
        {
            if (IsHandleCreated)
                Invoke(Close);
        }, cancellationToken);
        cancelableOperation.Start();

        var timeSpan = TimeSpan.FromMilliseconds(300);
        if (_showDialogDelay <= timeSpan)
        {
            cancelableOperation.Wait(_showDialogDelay);
        }
        else
        {
            cancelableOperation.Wait(timeSpan);
            if (!cancelableOperation.IsCompleted)
            {
                using (new CursorManager(Cursors.AppStarting))
                {
                    cancelableOperation.Wait(_showDialogDelay - timeSpan);
                }
            }
        }
    }

    private void ShowDialogIfNotCompleted(Task cancelableOperation)
    {
        if (!cancelableOperation.IsCompleted)
        {
            elapsedTimeTextBox.Text = GetElapsedText();
            StartTimer();
            ShowDialog(_owner);
        }
    }

    private void StartTimer()
    {
        var dueTime = TimeSpan.Zero;
        var period = TimeSpan.FromSeconds(1);
        _elapsedTimeTimer = new System.Threading.Timer(ElapsedTimeTimerCallback, null, dueTime, period);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        _elapsedTimeTimer!.Dispose();
    }

    private void ElapsedTimeTimerCallback(object? state)
    {
        if (IsHandleCreated)
        {
            var text = GetElapsedText();
            Invoke(() => { elapsedTimeTextBox.Text = text; });
        }
    }

    private string GetElapsedText()
    {
        var elapsed = Stopwatch.GetTimestamp() - _startTimestamp;
        var text = StopwatchTimeSpan.ToString(elapsed, 0);
        return text;
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        cancelButton.Enabled = false;
        textBox.AppendText("\r\nCanceling operation...");
        _cancellationTokenSource.Cancel();
    }
}