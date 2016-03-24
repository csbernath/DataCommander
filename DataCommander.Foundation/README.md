Foundation Class Library
========================

Collections
-----------

How to create memory indexes

|Unique|Sorted|Method|
|------|------|------|
|true|false|ToDictionary|
|false|false|ToLookup|
|true|true|AsReadOnlyDictionary|
|false|true|AsReadOnlyNonUniqueSortedList|

How to created large (segmented) collections

SegmentedArrayBuilder - build an array of segments

SegmentedCollection - build a linked list of segments

SegmentedListBuilder - build a list of segments
