namespace DataCommander.Foundation
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LeftClosedRightOpenIntervalComparer<T> : IIntervalComparer<T>
    {
        private readonly IComparer<T> comparer;

        /// <summary>
        /// 
        /// </summary>
        public LeftClosedRightOpenIntervalComparer()
        {
            this.comparer = Comparer<T>.Default;
        }
        
        bool IIntervalComparer<T>.IsValid(T left, T right)
        {
            return this.comparer.Compare(left, right) < 0;
        }

        bool IIntervalComparer<T>.Contains(T left, T value, T right)
        {
            return this.comparer.Compare(left, value) <= 0 && this.comparer.Compare(value, right) < 0;
        }

        bool IIntervalComparer<T>.Intersects(T left1, T right1, T left2, T right2)
        {
            return this.comparer.Compare(left2, right1) < 0 && this.comparer.Compare(left1, right2) < 0;
        }
    }
}