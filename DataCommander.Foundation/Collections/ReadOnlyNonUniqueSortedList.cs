namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ReadOnlyNonUniqueSortedList<TKey, TValue>
    {
        #region Private Fields

        private readonly IReadOnlyList<TValue> values;
        private readonly Func<TValue, TKey> keySelector;
        private readonly Comparison<TKey> comparison;
        private IReadOnlyList<int> groups;

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
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(comparison != null);
            Contract.Requires<ArgumentException>(
                values.SelectPreviousAndCurrentKey(keySelector).All(key => comparison(key.Previous, key.Current) <= 0),
                "keys must be ordered");

            this.values = values;
            this.keySelector = keySelector;
            this.comparison = comparison;

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
        public int Count => this.groups != null ? this.groups.Count : 0;

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
                Contract.Ensures(Contract.Result<IReadOnlyList<TValue>>() != null);

                IReadOnlyList<TValue> readOnlyList;

                int index = this.IndexOf(key);
                if (index >= 0)
                {
                    int currentGroupIndex = this.groups[index];
                    int nextGroupIndex = index < this.groups.Count - 1 ? this.groups[index + 1] : this.values.Count;
                    int count = nextGroupIndex - currentGroupIndex;

                    readOnlyList = new ReadOnlyListSegment<TValue>(this.values, currentGroupIndex, count);
                }
                else
                {
                    readOnlyList = EmptyReadOnlyList<TValue>.Instance;
                }

                return readOnlyList;
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
            if (this.values.Count > 0)
            {
                #region Create

                int notEqualsCount = this.values.SelectPreviousAndCurrentKey(this.keySelector).Count(k => comparison(k.Previous, k.Current) != 0);
                int smallArrayMaxLength = LargeObjectHeap.GetSmallArrayMaxLength(sizeof (int));
                int itemCount = notEqualsCount + 1;
                var segmentedArrayBuilder = new SegmentedArrayBuilder<int>(itemCount, smallArrayMaxLength);

                #endregion

                #region Fill

                segmentedArrayBuilder.Add(0);
                int index = 0;

                foreach (var key in this.values.SelectPreviousAndCurrentKey(this.keySelector))
                {
                    index++;

                    if (comparison(key.Previous, key.Current) != 0)
                    {
                        segmentedArrayBuilder.Add(index);
                    }
                }

                this.groups = segmentedArrayBuilder.ToReadOnlyList();

                #endregion
            }
        }

        [Pure]
        private int IndexOf(TKey key)
        {
            int index;

            if (this.groups != null)
            {
                index = BinarySearch.IndexOf(0, this.groups.Count - 1, currentIndex =>
                {
                    int valueIndex = this.groups[currentIndex];
                    var otherValue = this.values[valueIndex];
                    var otherKey = this.keySelector(otherValue);
                    return this.comparison(key, otherKey);
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