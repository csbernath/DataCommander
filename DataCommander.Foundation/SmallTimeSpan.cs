namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public struct SmallTimeSpan
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly SmallTimeSpan MinValue = new SmallTimeSpan(short.MinValue);

        /// <summary>
        /// 
        /// </summary>
        public static readonly SmallTimeSpan MaxValue = new SmallTimeSpan(short.MaxValue);

        private SmallTimeSpan(short value)
        {
            this.TotalMinutes = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        public SmallTimeSpan(TimeSpan timeSpan)
        {
            this.TotalMinutes = ToSmallTimeSpanValue(timeSpan);
        }

        /// <summary>
        /// 
        /// </summary>
        public short TotalMinutes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToTimeSpan(this.TotalMinutes).ToString();
        }

        private static TimeSpan ToTimeSpan(short value)
        {
            return TimeSpan.FromSeconds(value);
        }

        private static short ToSmallTimeSpanValue(TimeSpan timeSpan)
        {
            return (short)timeSpan.TotalSeconds;
        }
    }
}