namespace Foundation.Core.ClockAggregate
{
    public static class ClockAggregateRepository
    {
        private static ClockAggregateState _clockAggregateState = ClockAggregateRootFactory.Now().GetAggregateState();
        static ClockAggregateRepository() => ClockAggregateRepositoryUpdater.Start();
        public static void Save(ClockAggregateRoot clock) => _clockAggregateState = clock.GetAggregateState();
        public static ClockAggregateRoot Get() => new ClockAggregateRoot(_clockAggregateState);
    }
}