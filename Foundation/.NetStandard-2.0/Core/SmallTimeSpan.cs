using System;

namespace Foundation.Core
{
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
            TotalMinutes = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        public SmallTimeSpan(TimeSpan timeSpan)
        {
            TotalMinutes = ToSmallTimeSpanValue(timeSpan);
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
            return ToTimeSpan(TotalMinutes).ToString();
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