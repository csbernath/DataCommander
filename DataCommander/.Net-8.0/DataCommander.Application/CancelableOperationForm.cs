using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;
using Foundation.Assertions;
using Foundation.Core;

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
        var cancellationToken = _cancellationTokenSource.Token;
        cancelableOperation.ContinueWith(_ =>
        {
            if (IsHandleCreated)
                Invoke(Close);
        }, cancellationToken);
        _startTimestamp = Stopwatch.GetTimestamp();
        cancelableOperation.Start();
        var completed = cancelableOperation.Wait(_showDialogDelay);
        if (!completed)
        {
            var dueTime = TimeSpan.Zero;
            var period = TimeSpan.FromSeconds(1);
            _elapsedTimeTimer = new System.Threading.Timer(ElapsedTimeTimerCallback, null, dueTime, period);
            ShowDialog(_owner);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        _elapsedTimeTimer.Dispose();
    }

    private void ElapsedTimeTimerCallback(object? state)
    {
        if (IsHandleCreated)
        {
            var elapsed = Stopwatch.GetTimestamp() - _startTimestamp;
            var text = StopwatchTimeSpan.ToString(elapsed, 0);
            Invoke(() => { elapsedTimeTextBox.Text = text; });
        }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        cancelButton.Enabled = false;
        textBox.AppendText("\r\nCanceling operation...");
        _cancellationTokenSource.Cancel();
    }
}