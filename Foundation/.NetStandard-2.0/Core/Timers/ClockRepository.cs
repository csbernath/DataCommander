namespace Foundation.Core.Timers
{
    public static class ClockRepository
    {
        private static ClockState _clockState;
        public static void Save(Clock clock) => _clockState = clock.GetClockState();
        public static Clock Get() => new Clock(_clockState);
    }
}