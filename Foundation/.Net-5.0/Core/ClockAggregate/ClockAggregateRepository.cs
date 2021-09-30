namespace Foundation.Core.ClockAggregate
{
    public sealed class ClockAggregateRepository
    {
        public static ClockAggregateRepository Singleton = new();
        private ClockAggregateState _clockAggregateState;
        static ClockAggregateRepository() => ClockAggregateRepositoryUpdater.Start();
        private ClockAggregateRepository() => _clockAggregateState = ClockAggregateRootFactory.Now().GetAggregateState();
        internal void Save(ClockAggregateRoot clock) => _clockAggregateState = clock.GetAggregateState();
        public ClockAggregateRoot Get() => new(_clockAggregateState);
    }
}