namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public struct SmallTimeSpan
    {
        private short value;

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
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        public SmallTimeSpan(TimeSpan timeSpan)
        {
            this.value = ToSmallTimeSpanValue(timeSpan);
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