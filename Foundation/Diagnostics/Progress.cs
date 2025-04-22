using System;
using System.Diagnostics;
using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Diagnostics;

public class Progress
{
    private readonly int _startPercent;
    private readonly int _endPercent;
    private readonly int _taskCount;
    private readonly long _startTimestamp;
    private readonly Action<ProgressChangedEvent> _handleEvent;
    private int _currentTaskCount;
    private int _currentPercent;

    public Progress(int startPercent, int endPercent, int taskCount, long startTimestamp, Action<ProgressChangedEvent> handleEvent)
    {
        _startPercent = startPercent;
        _endPercent = endPercent;
        _taskCount = taskCount;
        _startTimestamp = startTimestamp;
        _handleEvent = handleEvent;

        _currentTaskCount = 0;
        _currentPercent = _startPercent;
    }

    public void Add(int value, long timestamp)
    {
        Assert.IsGreaterThanOrEqual(value, 0);
        var newTaskCount = _currentTaskCount + value;
        Assert.IsLessThanOrEqual(newTaskCount, _taskCount);

        var newRatio = (double)newTaskCount / _taskCount;
        var newPercentDouble = _startPercent + newRatio * (_endPercent - _startPercent);

        var elapsedTimeAmount = timestamp - _startTimestamp;
        var estimatedTimeAmount = (long)(elapsedTimeAmount / newRatio);
        var estimatedTimeAmountString = StopwatchTimeSpan.ToString(estimatedTimeAmount, 3);

        Debug.WriteLine(
            $"newTaskCount: {newTaskCount}, newPercentDouble: {newPercentDouble}, elapsed:  {StopwatchTimeSpan.ToString(elapsedTimeAmount, 3)} estimatedTimeAmount: {estimatedTimeAmountString}");

        var newPercent = (int)newPercentDouble;
        var changed = _currentPercent < newPercent;
        _currentPercent = newPercent;
        _currentTaskCount = newTaskCount;
        if (changed)
            _handleEvent(new ProgressChangedEvent(newTaskCount, newPercent));
    }
}