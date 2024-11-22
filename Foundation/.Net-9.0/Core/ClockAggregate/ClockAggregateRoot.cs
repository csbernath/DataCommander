using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate;

public class ClockAggregateRoot
{
    private readonly ClockAggregateState _clockAggregateState;

    internal ClockAggregateRoot(ClockAggregateState clockAggregateState)
    {
        Assert.IsNotNull(clockAggregateState);
        _clockAggregateState = clockAggregateState;
    }

    internal ClockAggregateState GetAggregateState() => _clockAggregateState;
}