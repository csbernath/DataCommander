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

###Configuration

The library defines a configuration section handler.
```
<configuration>
	<configSections>
		<section name="DataCommander.Foundation.Configuration" type="DataCommander.Foundation.Configuration.SectionHandler, DataCommander.Foundation"/>
	</configSections>
	<DataCommander.Foundation.Configuration>
	</DataCommander.Foundation.Configuration>
</configuration>
```

The schema of the configuration section is a tree of nodes. The node can contain child nodes and attributes.

```
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

```
<node name="<name>" description="<description>">
</node>
```

- name: optional
- description: optional

####Attributes

```
<attribute name="<name>" type="<type>" isNull="<isNull>" description="<description>"/>
```

- name: required
- type: optional (bool,char,string,object,sbyte,short,int,long,byte,ushort,uint,ulong,float,double,decimal,datetime,xmlnode,...)
- isNull: optional (true,false)
- description: optional

####Simplified syntax

```<node name="Name1">``` is equivalent to ```<Name1>```
```<attribute name="Name1" value="Hello">``` is equivalent to ```<attribute name="Name1" value="Hello">```

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
```
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

 
