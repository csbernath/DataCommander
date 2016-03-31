#Foundation Class Library

##Collections

###How to create memory indexes

Input: an enumerable item collection (sorted/not sorted, unique/not unique)

Output: a read only index of the input by a key

|Unique|Sorted|Create index method|Class implementing the index|
|------|------|-------------------|----------------------------|
|true|false|[`ToDictionary`](https://msdn.microsoft.com/en-us/library/system.linq.enumerable.todictionary(v=vs.110).aspx) |[`Dictionary<TKey,TValue>`](https://msdn.microsoft.com/en-us/library/xfhwa508(v=vs.110).aspx)
|false|false|[`ToLookup`](https://msdn.microsoft.com/en-us/library/system.linq.enumerable.tolookup(v=vs.110).aspx)|[`Lookup<TKey,TElement>`](https://msdn.microsoft.com/en-us/library/bb460184(v=vs.110).aspx)|
|true|true|[`AsReadOnlySortedList`](Linq/IEnumerableExtensions.cs)|[`ReadOnlySortedList<TKey,TValue>`](Collections/ReadOnlySortedList.cs)|
|false|true|[`AsReadOnlyNonUniqueSortedList`](Linq/IEnumerableExtensions.cs)|[`ReadOnlyNonUniqueSortedList<TKey,TValue>`](Collections/ReadOnlyNonUniqueSortedList.cs)

###How to create large (large object heap friendly, segmented) collections

SegmentedArrayBuilder - build an array of segments

SegmentedCollection - build a linked list of segments

SegmentedListBuilder - build a list of segments

|Method|SegmentedArrayBuilder|SegmentedCollection|SegmentedListBuilder|
|------|---------------------|-------------------|--------------------|
|number of items is known|yes|no|no|
|Add|yes|yes|yes|
|IEnumerable|no|yes|no|
|ToReadOnlyList|yes|no|yes|

###[IDateTimeProvider](IDateTimeProvider.cs)
This is the unified abstract version of retrieving the current date and time.
The concrete implementations are using the [`DateTime.Now`](https://msdn.microsoft.com/en-us/library/system.datetime.now(v=vs.110).aspx) and `DateTime.UtcNow` properties.
The implemenration uses the idea from [NLog's cached time source](https://github.com/NLog/NLog/blob/master/src/NLog/Time/CachedTimeSource.cs). The idea is that the Environment.TickCount is much faster then DateTime.(Utc)Now.
