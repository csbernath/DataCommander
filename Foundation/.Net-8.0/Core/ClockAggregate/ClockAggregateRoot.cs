using System;

namespace Foundation.Core.ClockAggregate;

public class ClockAggregateRoot
{
    private readonly ClockAggregateState _clockAggregateState;

    internal ClockAggregateRoot(ClockAggregateState clockAggregateState)
    {
        ArgumentNullException.ThrowIfNull(clockAggregateState);
        _clockAggregateState = clockAggregateState;
    }

    internal ClockAggregateState GetAggregateState() => _clockAggregateState;
}