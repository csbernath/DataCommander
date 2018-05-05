using System;
using System.Threading;
using Foundation.Assertions;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Int64PerformanceCounter
    {
        #region Private Fields

        private readonly string _name;
        private readonly Func<long, string> _toString;
        private long _count;
        private long _sum;
        private long _min = long.MaxValue;
        private long _max = long.MinValue;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="toString"></param>
        public Int64PerformanceCounter(string name, Func<long, string> toString)
        {
            Assert.IsNotNull(toString);

            _name = name;
            _toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Increment(long item)
        {
            Interlocked.Increment(ref _count);
            Interlocked.Add(ref _sum, item);

            while (true)
            {
                var min = _min;
                if (item < min)
                {
                    var originalMin = Interlocked.CompareExchange(ref _min, item, min);
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
                var max = _max;
                if (item > max)
                {
                    var originalMax = Interlocked.CompareExchange(ref _max, item, max);
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
        public long Count => _count;

        /// <summary>
        /// 
        /// </summary>
        public long Sum => _sum;

        /// <summary>
        /// 
        /// </summary>
        public long Min => _min;

        /// <summary>
        /// 
        /// </summary>
        public long Max => _max;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToLogString()
        {
            return $@"Int64PerformanceCounter '{_name}'
count: {_count}
min: {_toString(_min)}
avg: {_toString((long)((double)Sum / Count))}
max: {_toString(_max)}
sum: {_toString(_sum)}";
        }
    }
}