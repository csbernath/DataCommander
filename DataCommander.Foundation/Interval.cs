namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TIntervalComparer"></typeparam>
    public struct Interval<T, TIntervalComparer> where TIntervalComparer : IIntervalComparer<T>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly TIntervalComparer IntervalComparer = new TIntervalComparer();

        /// <summary>
        /// 
        /// </summary>
        public readonly T Left;

        /// <summary>
        /// 
        /// </summary>
        public readonly T Right;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Interval(T left, T right)
        {
            Contract.Requires<ArgumentException>(IntervalComparer.IsValid(left, right));

            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            return IntervalComparer.Contains(this.Left, value, this.Right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(Interval<T, TIntervalComparer> other)
        {
            return IntervalComparer.Intersects(this.Left, this.Right, other.Left, other.Right);
        }
    }
}