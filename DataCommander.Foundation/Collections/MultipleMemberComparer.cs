namespace DataCommander.Foundation.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MultipleMemberComparer<T> : IComparer<T>
    {
        private readonly IComparer<T>[] _comparers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparers"></param>
        public MultipleMemberComparer(params IComparer<T>[] comparers)
        {
            this._comparers = comparers;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            var result = 0;

            foreach (var comparer in this._comparers)
            {
                var currentResult = comparer.Compare(x, y);
                if (currentResult != 0)
                {
                    result = currentResult;
                    break;
                }
            }

            return result;
        }
    }
}