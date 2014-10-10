using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.InteropServices;
#if TEST
    using System.Runtime.CompilerServices;
#endif

[assembly: AssemblyConfiguration("Major")]
[assembly: AssemblyCopyright("Copyright (C) 2009-2014 DataCommander")]
[assembly: AssemblyCompany("DataCommander")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

#if FOUNDATION_3_5
    #if TEST
        [assembly: InternalsVisibleTo("UnitTestProject1, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a9e5f281a763b8b16ad5aca018ebf0a311bebd121abc862ca22df220c7e699b47c393f6c3e86ee294c3e23b2f1324fb721faf36abf0d55d5c9da803d8e2ef84a4652f1bee5527a147da2ea755e2d96018e3c446841355cc35150d57fc50be4c106b8155bfc564826461646caea80af4b85e06a406daa56a424f4da20036a93b8")]
    #else
        [assembly: AssemblyVersion( "6.0.0.0" )]
        [assembly: AssemblyTitle( "DataCommander.Foundation-3.5" )]
    #endif
#elif FOUNDATION_4_0
    #if TEST
        [assembly: InternalsVisibleTo("DataCommander.Foundation.Server-4.0, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a9e5f281a763b8b16ad5aca018ebf0a311bebd121abc862ca22df220c7e699b47c393f6c3e86ee294c3e23b2f1324fb721faf36abf0d55d5c9da803d8e2ef84a4652f1bee5527a147da2ea755e2d96018e3c446841355cc35150d57fc50be4c106b8155bfc564826461646caea80af4b85e06a406daa56a424f4da20036a93b8")]
    #else
        [assembly: AssemblyVersion("7.0.0.0")]
        [assembly: AssemblyTitle("DataCommander.Foundation-4.0")]
    #endif
#elif FOUNDATION_4_5
    #if !TEST
    [assembly: AssemblyVersion("1.0.0.0")]
    [assembly: AssemblyTitle("DataCommander.Foundation-4.5")]
    #endif
#endif

/*
7.0.0.0:
  - 2014.03.25: Adding IEnumerableExtensions.SelectIndexedItem method
  - 2014.03.17: Adding Factory.CreateCollection, CreateList, CreatehashSet methods
  - 2014.03.13: Removing FOUNDATION_2_0 (.NET 2.0 build)
  
6.0.0.0:
  - 2014.01.29 Diagnostics Assert, CollectionAssert, StringAssert classes are obsolete
  - 2014.01.28 Adding Microsoft.Contracts.dll reference
  - 2014.01.27 Adding EventableStream (e.g. cancellable Read)
  - 2014.01.24 Adding TaskMonitor
  - 2014.01.23 Adding CancellationTokenSource, CancellationToken
  - Adding Task, Task<T>
  - Adding SegmentedStreamReader 
  - Adding XmlSerializerExtensions.SerializeToXmlString 
  - Fixing bug in Parallel.Invoke  
  - Adding Document.GetOpenXmlPackageProperties method
  - Adding IDataRecordExtensions.GetValue(ordinal) method
  - Fixing bug in DataColumnSchema
  - Adding StreamExtensions
  - Fixing bugs in MemoryCache, DynamicArray
  - Removing Set (use HashSet in .NET 3.5)
  - Refactoring MemoryCache TimerCallback (ToArray -> ToDynamicArray)
  - Refactoring ICacheItem, CacheItem (GetValue property, SetValue method -> GetValue method)
  - Refactoring UniqueIndex, NonUniqueIndex (tryGetKey -> getKey, GetKeyResponse)
  - Removing internal SingleElementList
  - Refactoring ExecuteReader methods, adding IDataReaderContext
  - Adding IDbConnectionContext, IDbConnectionContextExtensions, DbConnectionContext
  - Removing obsolete methods
 
5.0.2.0:
    - Adding Win32ThreadId to WorkerThread start logging
    - Updating Dapper
    - Adding IndexedItem.Create method
    - Adding DynamicArray, SegmentCollection classes and IEnumerable<T> extension methods
    - Refactoring MemoryCache internals
    - Obsolete and new IDbConnection extension methods
    - Adding  IDataRecordExtensions.GetValueOrDefault<T>( this IDataRecord dataRecord, Int32 index ) method
    - Enhancing Document.ReadFromPackage method
    - Adding DocumentPropertyId.Manager
    - Adding IDbCommand ExecuteScalarValue, ExecuteScalarValueOrDefault extension methods 
    - Adding String Format extensions methods
  
5.0.1.0:
    - Adding TempFile class

5.0.0.0:
    - Refactoring logging system and log entry format
*/