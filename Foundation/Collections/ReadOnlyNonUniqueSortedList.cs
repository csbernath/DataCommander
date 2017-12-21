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
            FoundationContract.Requires<ArgumentNullException>(values != null);
            FoundationContract.Requires<ArgumentNullException>(keySelector != null);
            FoundationContract.Requires<ArgumentNullException>(comparison != null);
            FoundationContract.Requires<ArgumentException>(
                values.SelectPreviousAndCurrentKey(keySelector).All(key => comparison(key.Previous, key.Current) <= 0),
                "keys must be ordered");
#endif

            _values = values;
            _keySelector = keySelector;
            _comparison = comparison;

            InitializeGroups();
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
        public int Count => _groups?.Count ?? 0;

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

                var index = IndexOf(key);
                if (index >= 0)
                {
                    var currentGroupIndex = _groups[index];
                    var nextGroupIndex = index < _groups.Count - 1 ? _groups[index + 1] : _values.Count;
                    var count = nextGroupIndex - currentGroupIndex;

                    readOnlyList = new ReadOnlyListSegment<TValue>(_values, currentGroupIndex, count);
                }
                else
                    readOnlyList = EmptyReadOnlyList<TValue>.Value;

                return readOnlyList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IReadOnlyList<TValue>> GetGroups()
        {
            var lastGroupIndex = _groups.Count - 1;
            for (var groupIndex = 0; groupIndex <= lastGroupIndex; ++groupIndex)
            {
                var valueStartIndex = _groups[groupIndex];
                var valueNextStartIndex = groupIndex < lastGroupIndex ? _groups[groupIndex + 1] : _values.Count;
                var valueCount = valueNextStartIndex - valueStartIndex;
                yield return new ReadOnlyListSegment<TValue>(_values, valueStartIndex, valueCount);
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
            return IndexOf(key) >= 0;
        }

#endregion

#region Private Methods

        private void InitializeGroups()
        {
            if (_values.Count > 0)
            {
#region Create

                var notEqualsCount = _values.SelectPreviousAndCurrentKey(_keySelector).Count(k => _comparison(k.Previous, k.Current) != 0);
                var smallArrayMaxLength = LargeObjectHeap.GetSmallArrayMaxLength(sizeof(int));
                var itemCount = notEqualsCount + 1;
                var segmentedArrayBuilder = new SegmentedArrayBuilder<int>(itemCount, smallArrayMaxLength);

#endregion

#region Fill

                segmentedArrayBuilder.Add(0);
                var index = 0;

                foreach (var key in _values.SelectPreviousAndCurrentKey(_keySelector))
                {
                    index++;

                    if (_comparison(key.Previous, key.Current) != 0)
                    {
                        segmentedArrayBuilder.Add(index);
                    }
                }

                _groups = segmentedArrayBuilder.ToReadOnlyList();

#endregion
            }
        }

        [Pure]
        private int IndexOf(TKey key)
        {
            int index;

            if (_groups != null)
            {
                index = BinarySearch.IndexOf(0, _groups.Count - 1, currentIndex =>
                {
                    var valueIndex = _groups[currentIndex];
                    var otherValue = _values[valueIndex];
                    var otherKey = _keySelector(otherValue);
                    return _comparison(key, otherKey);
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