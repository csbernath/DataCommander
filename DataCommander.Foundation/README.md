#Foundation Class Library

##Collections

###How to create memory indexes for dynamic (not read only) collections

Input: a collection

Output: an indexable collection which follows add/remove operations in the indexes

See [```IndexableCollection<T>```](Collections/IndexableCollection/IndexableCollection-1.cs).

|Index class|Class implementing the index|Unique|Sorted|Enumerable|T this[int index]|
|-----------|----------------------------|------|------|----------|-----------------|
|[```LinkedListIndex```](Collections/IndexableCollection/LinkedListIndex.cs)|LinkedList<T>|false|false|true|false|
|[```ListIndex```](Collections/IndexableCollection/LinkedListIndex.cs)|List<T>|false|false|true|true|
|[```NonUniqueIndex```](Collections/IndexableCollection/NonUniqueIndex.cs)|IDictionary<TKey, ICollection<T>>|false|-|false|
|[```SequenceIndex```](Collections/IndexableCollection/SequenceIndex.cs)|IDictionary<>|true|true/false|false|
|[```UniqueIndex```](Collections/IndexableCollection/UniqueIndex.cs)|Dictionary<>|false|false|false|
|```UniqueIndex```|SortedDictionary<>|false|true|false|
|[```UniqueListIndex```](Collections/IndexableCollection/UniqueListIndex.cs)|IList<T>|true|true/false|true|

###How to create memory indexes for read only collections

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

###Configuration

The library defines a configuration section handler.
```xml
<configuration>
	<configSections>
		<section name="DataCommander.Foundation.Configuration" type="DataCommander.Foundation.Configuration.SectionHandler, DataCommander.Foundation"/>
	</configSections>
	<DataCommander.Foundation.Configuration>
	</DataCommander.Foundation.Configuration>
</configuration>
```

The schema of the configuration section is a tree of nodes. The node can contain child nodes and attributes.

```xml
<DataCommander.Foundation.Configuration>
	<node name="Node1">
		<attribute name="Enabled" type="bool" value="true"/>
		<attribute name="Path" value="%TEMP%"/>
	</node>
	<Node2>
	</Node2>
</DataCommander.Foundation.Configuration>
</configuration>
```

Reserved xml element names: node, attribute.
If the name of the xml element is not node or attribute then the type of the element is node and the name of the node is the name of the xml element.

####Nodes

```xml
<node name="<name>" description="<description>">
</node>
```

- name: optional
- description: optional

####Attributes

```xml
<attribute name="<name>" type="<type>" isNull="<isNull>" description="<description>" value="<value>"/>
```

- name: required
- type: optional (the full type name of the .NET type)
- isNull: optional (true,false)
- description: optional
- value: required

#####Array value

```xml
<attribute name="MyArray" type="int[]">
	<a value="3"/>
	<a value="5"/>
	<a value="7"/>
	<a value="9"/>
</attribute>
```

- the name of the of the array item xml element: required, arbitrary (in this sample: 'a')
- value: required (3,5,7,9)

#####Byte array value

```xml
<attribute name="EncryptedPassword" type="byte[]">
VGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhp
cyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBp
cyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBh
IHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNl
Y3JldCBwYXNzd29yZA0KVGhpcyBpcyBhIHNlY3JldCBwYXNzd29yZA==
</attribute>
```

####Simplified syntax

|Full syntax|Simplified syntax|
|-----------|-----------------|
|```<node name="Name1">```|```<Name1>```|
|```<attribute name="Count" type="System.Int32" value="1"/>```|```<attribute name="Count" type="int" value="1"/>``` |
|```<attribute name="Name1" type="string" value="Hello">```|```<attribute name="Name1" value="Hello">```|

C# type names:
  - bool
  - char
  - string
  - sbyte
  - short
  - int
  - long
  - byte
  - ushort
  - uint
  - ulong
  - float
  - double
  - decimal

Additional reserved type names:
   - datetime
   - xmlnode

###[IDateTimeProvider](IDateTimeProvider.cs)
This is the unified abstract version of retrieving the current date and time.
The concrete implementations are using the [`DateTime.Now`](https://msdn.microsoft.com/en-us/library/system.datetime.now(v=vs.110).aspx) and `DateTime.UtcNow` properties.
The implementation uses the idea from [NLog's cached time source](https://github.com/NLog/NLog/blob/master/src/NLog/Time/CachedTimeSource.cs). The idea is that the Environment.TickCount is much faster then DateTime.(Utc)Now.

###Logging
The library defines interfaces for logging:
  - [`ILog`](Diagnostics/Log/ILog.cs)
  - [`ILogFactory`](Diagnostics/Log/ILogFactory.cs)

The library contains an implementation of the interfaces:
  - [`FoundationLog`](Diagnostics/Log/FoundationLog.cs)
  - [`FoundationLogFactory`](Diagnostics/Log/FoundationLogFactory.cs)

The built-in implementation can be replaced with NLog,log4net etc. The application configuration file can be used for that.
See the default configuration in Data Commander as an example:
```xml
<Diagnostics>
	<LogFactory>
		<attribute name="TypeName" value="DataCommander.Foundation.Diagnostics.FoundationLogFactory"/>
	</LogFactory>
	<FoundationLogFactory>
		<attribute name="DateTimeKind" type="System.DateTimeKind" value="Local"/>
		<LogWriters>
			<node>
				<attribute name="Type" value="FileLogWriter"/>
				<attribute name="Enabled" type="bool" value="true"/>
				<attribute name="LogLevel" type="DataCommander.Foundation.Diagnostics.LogLevel" value="Debug"/>
				<attribute name="DateTimeKind" type="System.DateTimeKind" value="Local"/>
				<attribute name="Path" value="%TEMP%\DataCommander[{date}]({time}) {guid}.log"/>
				<attribute name="Async" type="bool" value="true"/>
				<attribute name="FileAttributes" type="System.IO.FileAttributes" value="ReadOnly,Hidden"/>
			</node>
		</LogWriters>
	</FoundationLogFactory>
</Diagnostics>
```
The log factory class is the built-in FoundationLogFactory. The configuration of this class is in the FoundationLogFactory xml element. The factory creates a file log writer. The log files will be written into the %TEMP% directory. One file per application start.

 
