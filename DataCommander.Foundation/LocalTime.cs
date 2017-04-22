namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class LocalTime : IDateTimeProvider
    {
        private readonly int increment;
        private readonly int adjustment;

        private int lastTickCount;
        private DateTime lastDateTime;

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

            this.lastTickCount = UniversalTime.GetTickCount();
            this.lastDateTime = DateTime.Now;
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
                var lastTickCount = this.lastTickCount;
                var lastDateTime = this.lastDateTime;
                var tickCount = UniversalTime.GetTickCount();
                var elapsed = tickCount - lastTickCount;
                DateTime now;

                if (this.increment <= elapsed)
                {
                    if (elapsed < this.adjustment)
                        now = lastDateTime.AddMilliseconds(elapsed);
                    else
                    {
                        now = DateTime.Now;
                        this.lastTickCount = tickCount;
                        this.lastDateTime = now;
                    }
                }
                else
                    now = lastDateTime;

                return now;
            }
        }
    }
}