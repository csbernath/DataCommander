using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

public sealed class ReadOnlyNonUniqueSortedList<TKey, TValue>
{
    private readonly IReadOnlyList<TValue> _values;
    private readonly Func<TValue, TKey> _keySelector;
    private readonly Comparison<TKey> _comparison;
    private IReadOnlyList<int> _groups;

    public ReadOnlyNonUniqueSortedList(IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector, Comparison<TKey> comparison)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparison);
        Assert.ArgumentConditionIsTrue(
            values.SelectPreviousAndCurrentKey(keySelector).All(key => comparison(key.Previous, key.Current) <= 0),
            "keys must be ordered");

        _values = values;
        _keySelector = keySelector;
        _comparison = comparison;

        InitializeGroups();
    }

    public ReadOnlyNonUniqueSortedList(IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector) : this(values, keySelector,
        Comparer<TKey>.Default.Compare)
    {
    }

    [Pure]
    public int Count => _groups?.Count ?? 0;

    [Pure]
    public IReadOnlyList<TValue> this[TKey key]
    {
        get
        {
            IReadOnlyList<TValue> readOnlyList;
            int index = IndexOf(key);
            if (index >= 0)
            {
                int currentGroupIndex = _groups[index];
                int nextGroupIndex = index < _groups.Count - 1 ? _groups[index + 1] : _values.Count;
                int count = nextGroupIndex - currentGroupIndex;

                readOnlyList = new ReadOnlyListSegment<TValue>(_values, currentGroupIndex, count);
            }
            else
                readOnlyList = EmptyReadOnlyCollection<TValue>.Value;

            ArgumentNullException.ThrowIfNull(readOnlyList);
            return readOnlyList;
        }
    }

    [Pure]
    public bool ContainsKey(TKey key) => IndexOf(key) >= 0;

    public IEnumerable<IReadOnlyList<TValue>> GetGroups()
    {
        int lastGroupIndex = _groups.Count - 1;
        for (int groupIndex = 0; groupIndex <= lastGroupIndex; ++groupIndex)
        {
            int valueStartIndex = _groups[groupIndex];
            int valueNextStartIndex = groupIndex < lastGroupIndex ? _groups[groupIndex + 1] : _values.Count;
            int valueCount = valueNextStartIndex - valueStartIndex;
            yield return new ReadOnlyListSegment<TValue>(_values, valueStartIndex, valueCount);
        }
    }

    private void InitializeGroups()
    {
        if (_values.Count > 0)
        {
            int notEqualsCount = _values.SelectPreviousAndCurrentKey(_keySelector).Count(k => _comparison(k.Previous, k.Current) != 0);
            int smallArrayMaxLength = LargeObjectHeap.GetSmallArrayMaxLength(sizeof(int));
            int itemCount = notEqualsCount + 1;
            SegmentedArrayBuilder<int> segmentedArrayBuilder = new SegmentedArrayBuilder<int>(itemCount, smallArrayMaxLength);

            segmentedArrayBuilder.Add(0);
            int index = 0;

            foreach (PreviousAndCurrent<TKey> key in _values.SelectPreviousAndCurrentKey(_keySelector))
            {
                index++;

                if (_comparison(key.Previous, key.Current) != 0) segmentedArrayBuilder.Add(index);
            }

            _groups = segmentedArrayBuilder.ToReadOnlyCollection();
        }
    }

    [Pure]
    private int IndexOf(TKey key)
    {
        int index;

        if (_groups != null)
            index = BinarySearch.IndexOf(0, _groups.Count - 1, currentIndex =>
            {
                int valueIndex = _groups[currentIndex];
                TValue otherValue = _values[valueIndex];
                TKey otherKey = _keySelector(otherValue);
                return _comparison(key, otherKey);
            });
        else
            index = -1;

        return index;
    }
}