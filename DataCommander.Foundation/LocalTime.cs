namespace DataCommander.Foundation
{
    using System;

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
            Contract.Requires<ArgumentOutOfRangeException>(increment >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(increment <= adjustment);
#endif

            this._increment = increment;
            this._adjustment = adjustment;

            this._lastTickCount = UniversalTime.GetTickCount();
            this._lastDateTime = DateTime.Now;
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
                var lastTickCount = this._lastTickCount;
                var lastDateTime = this._lastDateTime;
                var tickCount = UniversalTime.GetTickCount();
                var elapsed = tickCount - lastTickCount;
                DateTime now;

                if (this._increment <= elapsed)
                {
                    if (elapsed < this._adjustment)
                        now = lastDateTime.AddMilliseconds(elapsed);
                    else
                    {
                        now = DateTime.Now;
                        this._lastTickCount = tickCount;
                        this._lastDateTime = now;
                    }
                }
                else
                    now = lastDateTime;

                return now;
            }
        }
    }
}