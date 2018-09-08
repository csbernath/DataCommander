using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation
{
    public sealed class UniversalTime : IDateTimeProvider
    {
        private static volatile int _sharedTickCount;
        private static DateTime _sharedDateTime;

        private readonly int _increment;
        private readonly int _adjustment;

        private int _incrementedTickCount;
        private DateTime _incrementedDateTime;

        static UniversalTime()
        {
            _sharedTickCount = Environment.TickCount;
            _sharedDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="increment">increment interval in milliseconds</param>
        /// <param name="adjustment">adjustement interval in millseconds</param>
        public UniversalTime(int increment, int adjustment)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);

            _increment = increment;
            _adjustment = adjustment;

            _sharedDateTime = DateTime.Now;

            _incrementedTickCount = _sharedTickCount;
            _incrementedDateTime = _sharedDateTime;
        }

        public static int TickCount => _sharedTickCount;

        public static UniversalTime Default { get; } = new UniversalTime(increment: 16, adjustment: 60 * 1000);

        public static int GetTickCount()
        {
            _sharedTickCount = Environment.TickCount;
            return _sharedTickCount;
        }

        /// <summary>
        /// Gets the current date and time on this computer, expressed as the local time.
        /// The system clock resolution can be 1.000 - 15.600 millseconds.
        /// </summary>
        public DateTime UtcNow
        {
            get
            {
                var elapsed = GetTickCount() - _incrementedTickCount;
                if (_increment <= elapsed)
                {
                    if (elapsed < _adjustment)
                    {
                        var calculatedDateTime = _incrementedDateTime.AddMilliseconds(elapsed);
                        if (_sharedDateTime < calculatedDateTime)
                            _sharedDateTime = calculatedDateTime;
                    }
                    else
                        _sharedDateTime = DateTime.UtcNow;

                    _incrementedTickCount = _sharedTickCount;
                    _incrementedDateTime = _sharedDateTime;
                }

                return _sharedDateTime;
            }
        }

        DateTime IDateTimeProvider.Now => Default.UtcNow;
    }
}