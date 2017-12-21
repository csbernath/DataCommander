using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UniversalTime : IDateTimeProvider
    {
        private static volatile int sharedTickCount;
        private static DateTime sharedDateTime;

        private readonly int increment;
        private readonly int adjustment;

        private int incrementedTickCount;
        private DateTime incrementedDateTime;

        static UniversalTime()
        {
            sharedTickCount = Environment.TickCount;
            sharedDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="increment">increment interval in milliseconds</param>
        /// <param name="adjustment">adjustement interval in millseconds</param>
        public UniversalTime(int increment, int adjustment)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);
#endif

            this.increment = increment;
            this.adjustment = adjustment;

            sharedDateTime = DateTime.Now;

            incrementedTickCount = sharedTickCount;
            incrementedDateTime = sharedDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int TickCount => sharedTickCount;

        /// <summary>
        /// 
        /// </summary>
        public static UniversalTime Default { get; } = new UniversalTime(increment: 16, adjustment: 60*1000);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetTickCount()
        {
            sharedTickCount = Environment.TickCount;
            return sharedTickCount;
        }

        /// <summary>
        /// Gets the current date and time on this computer, expressed as the local time.
        /// The system clock resolution can be 1.000 - 15.600 millseconds.
        /// </summary>
        public DateTime UtcNow
        {
            get
            {
                var elapsed = GetTickCount() - incrementedTickCount;
                if (increment <= elapsed)
                {
                    if (elapsed < adjustment)
                    {
                        var calculatedDateTime = incrementedDateTime.AddMilliseconds(elapsed);
                        if (sharedDateTime < calculatedDateTime)
                        {
                            sharedDateTime = calculatedDateTime;
                        }
                    }
                    else
                    {
                        sharedDateTime = DateTime.UtcNow;
                    }

                    incrementedTickCount = sharedTickCount;
                    incrementedDateTime = sharedDateTime;
                }

                return sharedDateTime;
            }
        }

        DateTime IDateTimeProvider.Now => Default.UtcNow;
    }
}