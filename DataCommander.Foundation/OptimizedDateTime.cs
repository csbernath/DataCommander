namespace DataCommander.Foundation
{
    using System;

    //public sealed class Timestamp
    //{
    //    public Int32 EnvironmentTickCount;
    //    public DateTime DateTime;
    //    public Int64 StopwatchTimestamp;

    //    public static OptimizedTimeSpan operator -( Timestamp end, Timestamp start )
    //    {
    //        return new OptimizedTimeSpan
    //        {
    //            EnvironmentTickCount = end.EnvironmentTickCount - start.EnvironmentTickCount,
    //            StopwatchTimestamp = end.StopwatchTimestamp - start.StopwatchTimestamp,
    //            TimeSpan = end.DateTime - start.DateTime
    //        };
    //    }
    //}

    //public sealed class OptimizedTimeSpan
    //{
    //    public Int32 EnvironmentTickCount;
    //    public TimeSpan TimeSpan;
    //    public Int64 StopwatchTimestamp;
    //}

    /// <summary>
    /// 
    /// </summary>
    public static class OptimizedDateTime
    {
        private static Int32 tickCount;
        private static DateTime now;

        /// <summary>
        /// 
        /// </summary>
        public static DateTime Now
        {
            get
            {
                Int32 tickCount = Environment.TickCount;
                if (tickCount != OptimizedDateTime.tickCount)
                {
                    OptimizedDateTime.tickCount = tickCount;
                    now = DateTime.Now;
                }
                return now;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNow()
        {
            tickCount = Environment.TickCount;
            now = DateTime.Now;
            return now;
        }
    }
}