namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public struct SmallTimeSpan
    {
        private readonly short value;

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

        /// <summary>
        /// 
        /// </summary>
        public short TotalMinutes
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToTimeSpan(this.value).ToString();
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