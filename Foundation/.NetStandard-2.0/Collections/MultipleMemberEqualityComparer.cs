﻿using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public sealed class MultipleMemberEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Private Fields

        private readonly IEqualityComparer<T>[] _equalityComparers;

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="equalityComparers"></param>
        public MultipleMemberEqualityComparer(params IEqualityComparer<T>[] equalityComparers)
        {
            Assert.IsNotNull(equalityComparers);
            Assert.IsInRange(equalityComparers.Length > 0);
            //Assert.IsNotNull(Contract.ForAll(equalityComparers, c => c != null));

            _equalityComparers = equalityComparers;
        }

        #endregion

        #region Private Methods

        private static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }

        #endregion

        #region IEqualityComparer<T> Members

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return _equalityComparers.All(c => c.Equals(x, y));
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            var hashCodes = _equalityComparers.Select(c => c.GetHashCode(obj));
            var hashCode = hashCodes.Aggregate(CombineHashCodes);
            return hashCode;
        }

        #endregion
    }
}