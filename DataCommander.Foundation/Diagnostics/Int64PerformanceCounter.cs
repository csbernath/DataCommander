namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Threading;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(toString != null);
#endif

            this.name = name;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Increment(long item)
        {
            Interlocked.Increment(ref this.count);
            Interlocked.Add(ref this.sum, item);

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
        public long Count => this.count;

        /// <summary>
        /// 
        /// </summary>
        public long Sum => this.sum;

        /// <summary>
        /// 
        /// </summary>
        public long Min => this.min;

        /// <summary>
        /// 
        /// </summary>
        public long Max => this.max;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToLogString()
        {
            return $@"Int64PerformanceCounter '{this.name}'
count: {this.count}
min: {this.toString(this.min)}
avg: {this.toString((long)((double)this.Sum/this.Count))}
max: {this.toString(this.max)}
sum: {this.toString(this.sum)}";
        }
    }
}