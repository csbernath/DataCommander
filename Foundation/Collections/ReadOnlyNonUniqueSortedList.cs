using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Linq;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ReadOnlyNonUniqueSortedList<TKey, TValue>
    {
        #region Private Fields

        private readonly IReadOnlyList<TValue> _values;
        private readonly Func<TValue, TKey> _keySelector;
        private readonly Comparison<TKey> _comparison;
        private IReadOnlyList<int> _groups;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparison"></param>
        public ReadOnlyNonUniqueSortedList(
            IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector,
            Comparison<TKey> comparison)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(comparison != null);
            Contract.Requires<ArgumentException>(
                values.SelectPreviousAndCurrentKey(keySelector).All(key => comparison(key.Previous, key.Current) <= 0),
                "keys must be ordered");
#endif

            this._values = values;
            this._keySelector = keySelector;
            this._comparison = comparison;

            this.InitializeGroups();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        public ReadOnlyNonUniqueSortedList(
            IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector)
            : this(values, keySelector, Comparer<TKey>.Default.Compare)
        {
        }

#endregion

#region Public Properties

        /// <summary>
        /// 
        /// </summary>
        [Pure]
        public int Count => this._groups?.Count ?? 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Pure]
        public IReadOnlyList<TValue> this[TKey key]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Ensures(Contract.Result<IReadOnlyList<TValue>>() != null);
#endif

                IReadOnlyList<TValue> readOnlyList;

                var index = this.IndexOf(key);
                if (index >= 0)
                {
                    var currentGroupIndex = this._groups[index];
                    var nextGroupIndex = index < this._groups.Count - 1 ? this._groups[index + 1] : this._values.Count;
                    var count = nextGroupIndex - currentGroupIndex;

                    readOnlyList = new ReadOnlyListSegment<TValue>(this._values, currentGroupIndex, count);
                }
                else
                {
                    readOnlyList = EmptyReadOnlyList<TValue>.Instance;
                }

                return readOnlyList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IReadOnlyList<TValue>> GetGroups()
        {
            var lastGroupIndex = this._groups.Count - 1;
            for (var groupIndex = 0; groupIndex <= lastGroupIndex; ++groupIndex)
            {
                var valueStartIndex = this._groups[groupIndex];
                var valueNextStartIndex = groupIndex < lastGroupIndex ? this._groups[groupIndex + 1] : this._values.Count;
                var valueCount = valueNextStartIndex - valueStartIndex;
                yield return new ReadOnlyListSegment<TValue>(this._values, valueStartIndex, valueCount);
            }
        }

#endregion

#region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Pure]
        public bool ContainsKey(TKey key)
        {
            return this.IndexOf(key) >= 0;
        }

#endregion

#region Private Methods

        private void InitializeGroups()
        {
            if (this._values.Count > 0)
            {
#region Create

                var notEqualsCount = this._values.SelectPreviousAndCurrentKey(this._keySelector).Count(k => _comparison(k.Previous, k.Current) != 0);
                var smallArrayMaxLength = LargeObjectHeap.GetSmallArrayMaxLength(sizeof(int));
                var itemCount = notEqualsCount + 1;
                var segmentedArrayBuilder = new SegmentedArrayBuilder<int>(itemCount, smallArrayMaxLength);

#endregion

#region Fill

                segmentedArrayBuilder.Add(0);
                var index = 0;

                foreach (var key in this._values.SelectPreviousAndCurrentKey(this._keySelector))
                {
                    index++;

                    if (_comparison(key.Previous, key.Current) != 0)
                    {
                        segmentedArrayBuilder.Add(index);
                    }
                }

                this._groups = segmentedArrayBuilder.ToReadOnlyList();

#endregion
            }
        }

        [Pure]
        private int IndexOf(TKey key)
        {
            int index;

            if (this._groups != null)
            {
                index = BinarySearch.IndexOf(0, this._groups.Count - 1, currentIndex =>
                {
                    var valueIndex = this._groups[currentIndex];
                    var otherValue = this._values[valueIndex];
                    var otherKey = this._keySelector(otherValue);
                    return this._comparison(key, otherKey);
                });
            }
            else
            {
                index = -1;
            }

            return index;
        }

#endregion
    }
}