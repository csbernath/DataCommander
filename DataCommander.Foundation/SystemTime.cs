namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SystemTime
    {
        private static volatile int sharedTickCount;
        private static DateTime sharedDateTime;

        private readonly int increment;
        private readonly int adjustment;

        private int incrementedTickCount;
        private DateTime incrementedDateTime;

        private static SystemTime defaultSystemTime = new SystemTime(increment: 16, adjustment: 60*1000);

        static SystemTime()
        {
            sharedTickCount = Environment.TickCount;
            sharedDateTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="increment">increment interval in milliseconds</param>
        /// <param name="adjustment">adjustement interval in millseconds</param>
        public SystemTime(int increment, int adjustment)
        {
            Contract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);

            this.increment = increment;
            this.adjustment = adjustment;

            sharedDateTime = DateTime.Now;

            this.incrementedTickCount = sharedTickCount;
            this.incrementedDateTime = sharedDateTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int TickCount
        {
            get
            {
                return sharedTickCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static SystemTime Default
        {
            get
            {
                return defaultSystemTime;
            }
        }

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
                int elapsed = GetTickCount() - this.incrementedTickCount;
                if (this.increment <= elapsed)
                {
                    if (elapsed < this.adjustment)
                    {
                        DateTime calculatedDateTime = this.incrementedDateTime.AddMilliseconds(elapsed);
                        if (sharedDateTime < calculatedDateTime)
                        {
                            sharedDateTime = calculatedDateTime;
                        }
                    }
                    else
                    {
                        sharedDateTime = DateTime.UtcNow;
                    }

                    this.incrementedTickCount = sharedTickCount;
                    this.incrementedDateTime = sharedDateTime;
                }

                return sharedDateTime;
            }
        }
    }
}