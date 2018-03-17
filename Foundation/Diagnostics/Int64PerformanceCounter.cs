using System;
using System.Threading;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Int64PerformanceCounter
    {
        #region Private Fields

        private readonly string name;
        private readonly Func<long, string> toString;
        private long count;
        private long sum;
        private long min = long.MaxValue;
        private long max = long.MinValue;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="toString"></param>
        public Int64PerformanceCounter(string name, Func<long, string> toString)
        {
            Assert.IsNotNull(toString);

            this.name = name;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Increment(long item)
        {
            Interlocked.Increment(ref count);
            Interlocked.Add(ref sum, item);

            while (true)
            {
                var min = this.min;
                if (item < min)
                {
                    var originalMin = Interlocked.CompareExchange(ref this.min, item, min);
                    if (originalMin == min)
                    {
                        break;
                    }
                    else
                    {
                        Thread.SpinWait(1);
                    }
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                var max = this.max;
                if (item > max)
                {
                    var originalMax = Interlocked.CompareExchange(ref this.max, item, max);
                    if (originalMax == max)
                    {
                        break;
                    }
                    else
                    {
                        Thread.SpinWait(1);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Count => count;

        /// <summary>
        /// 
        /// </summary>
        public long Sum => sum;

        /// <summary>
        /// 
        /// </summary>
        public long Min => min;

        /// <summary>
        /// 
        /// </summary>
        public long Max => max;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToLogString()
        {
            return $@"Int64PerformanceCounter '{name}'
count: {count}
min: {toString(min)}
avg: {toString((long)((double)Sum / Count))}
max: {toString(max)}
sum: {toString(sum)}";
        }
    }
}