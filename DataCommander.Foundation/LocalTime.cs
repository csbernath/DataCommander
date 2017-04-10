namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LocalTime : IDateTimeProvider
    {
        private static DateTime sharedDateTime;

        private readonly int increment;
        private readonly int adjustment;

        private int incrementedTickCount;
        private DateTime incrementedDateTime;

        static LocalTime()
        {
            sharedDateTime = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="increment"></param>
        /// <param name="adjustment"></param>
        public LocalTime(int increment, int adjustment)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);
#endif

            this.increment = increment;
            this.adjustment = adjustment;

            sharedDateTime = DateTime.Now;

            this.incrementedTickCount = UniversalTime.TickCount;
            this.incrementedDateTime = sharedDateTime;
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
                var elapsed = UniversalTime.GetTickCount() - this.incrementedTickCount;
                if (this.increment <= elapsed)
                {
                    if (elapsed < this.adjustment)
                    {
                        var calculatedDateTime = this.incrementedDateTime.AddMilliseconds(elapsed);
                        if (sharedDateTime < calculatedDateTime)
                        {
                            sharedDateTime = calculatedDateTime;
                        }
                    }
                    else
                    {
                        sharedDateTime = DateTime.Now;
                    }

                    this.incrementedTickCount = UniversalTime.TickCount;
                    this.incrementedDateTime = sharedDateTime;
                }

                return sharedDateTime;
            }
        }
    }
}