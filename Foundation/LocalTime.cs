using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LocalTime : IDateTimeProvider
    {
        private readonly int _increment;
        private readonly int _adjustment;

        private int _lastTickCount;
        private DateTime _lastDateTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="increment"></param>
        /// <param name="adjustment"></param>
        public LocalTime(int increment, int adjustment)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);
#endif

            _increment = increment;
            _adjustment = adjustment;

            _lastTickCount = UniversalTime.GetTickCount();
            _lastDateTime = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        public static LocalTime Default { get; } = new LocalTime(increment: 16, adjustment: 1000);

        /// <summary>
        /// Gets the current date and time on this computer, expressed as the local time.
        /// The system clock resolution can be 1.000 - 15.600 millseconds.
        /// </summary>
        public DateTime Now
        {
            get
            {
                var lastTickCount = _lastTickCount;
                var lastDateTime = _lastDateTime;
                var tickCount = UniversalTime.GetTickCount();
                var elapsed = tickCount - lastTickCount;
                DateTime now;

                if (_increment <= elapsed)
                {
                    if (elapsed < _adjustment)
                        now = lastDateTime.AddMilliseconds(elapsed);
                    else
                    {
                        now = DateTime.Now;
                        _lastTickCount = tickCount;
                        _lastDateTime = now;
                    }
                }
                else
                    now = lastDateTime;

                return now;
            }
        }
    }
}