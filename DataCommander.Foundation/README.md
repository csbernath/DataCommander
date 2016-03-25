#Foundation Class Library

##Collections

###How to create memory indexes

|Unique|Sorted|Create index method|Class implementing the index|
|------|------|-------------------|----------------------------|
|true|false|[System.Linq.Enumerable.ToDictionary](https://msdn.microsoft.com/en-us/library/system.linq.enumerable.todictionary(v=vs.110).aspx) |[System.Collections.Generic.Dictionary<TKey,TValue>](https://msdn.microsoft.com/en-us/library/xfhwa508(v=vs.110).aspx)
|false|false|ToLookup (Framework)|
|true|true|AsReadOnlySortedList (Foundation)|
|false|true|AsReadOnlyNonUniqueSortedList (Foundation)|

###How to created large (segmented) collections

SegmentedArrayBuilder - build an array of segments

SegmentedCollection - build a linked list of segments

SegmentedListBuilder - build a list of segments

|Method|SegmentedArrayBuilder|SegmentedCollection|SegmentedListBuilder|
|------|---------------------|-------------------|--------------------|
|number of items is known|yes|no|no|
|Add|yes|yes|yes|
|IEnumerable|no|yes|no|
|ToReadOnlyList|yes|no|yes|
