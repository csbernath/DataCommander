using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Api;
using DataCommander.Application.Connection;
using Foundation.Core;
using Foundation.Log;

namespace DataCommander.Application;

public partial class CancelableOperationForm : Form
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();    
    private readonly Control _owner;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private System.Threading.Timer? _elapsedTimeTimer;
    private long _startTimestamp;
    private long _elapsedTicks;
    private bool _operationCanceled;

    public CancelableOperationForm(
        Control owner,
        CancellationTokenSource cancellationTokenSource,
        string formText,
        string textBoxText,
        ColorTheme? colorTheme)
    {
        ArgumentNullException.ThrowIfNull(owner);
        _owner = owner;
        _cancellationTokenSource = cancellationTokenSource;
        _elapsedTimeTimer = new System.Threading.Timer(ElapsedTimeTimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        InitializeComponent();
        colorTheme?.Apply(this);
        Text = formText;
        textBox.AppendText(textBoxText);
    }

    public bool OperationCanceled => _operationCanceled;
    public long ElapsedTicks => _elapsedTicks;

    public void OpenForm(TimeSpan delay)
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        var cancellationToken = _cancellationTokenSource.Token;
        Task.Delay(delay, cancellationToken)
            .ContinueWith(_ =>
            {
                var period = TimeSpan.FromSeconds(1);
                if (_elapsedTimeTimer != null && !cancellationToken.IsCancellationRequested)
                {
                    _elapsedTimeTimer.Change(TimeSpan.Zero, period);
                    _owner.Invoke(() => ShowDialog(_owner));
                }
            }, cancellationToken);
    }

    public void CloseForm()
    {
        _elapsedTicks = Stopwatch.GetTimestamp() - _startTimestamp;

        if (_elapsedTimeTimer != null)
        {
            _elapsedTimeTimer.Dispose();
            _elapsedTimeTimer = null;
        }

        if (IsHandleCreated)
            Invoke(Close);
    }

    private void ElapsedTimeTimerCallback(object? state)
    {
        if (IsHandleCreated)
        {
            var elapsed = Stopwatch.GetTimestamp() - _startTimestamp;
            Invoke(() => { elapsedTimeTextBox.Text = StopwatchTimeSpan.ToString(elapsed, 0); });
        }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        cancelButton.Enabled = false;
        _operationCanceled = true;
        textBox.AppendText("\r\nCanceling action...");
        _cancellationTokenSource.Cancel();
    }
}