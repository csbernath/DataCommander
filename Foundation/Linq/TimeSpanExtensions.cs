using System;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static double GetTotalMicroseconds(this TimeSpan timeSpan)
        {
            return timeSpan.Ticks*0.1;
        }
    }
}