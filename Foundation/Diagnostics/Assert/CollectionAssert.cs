#if FOUNDATION_3_5

namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public static class CollectionAssert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="collectionName"></param>
        /// <param name="itemName"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void Contains<T>(
            ICollection<T> collection,
            T item,
            string collectionName,
            string itemName )
        {
            FoundationContract.Requires<ArgumentNullException>( collection != null );
            bool contains = collection.Contains( item );
            if (!contains)
            {
                AssertMessage message = new AssertMessage( "CollectionAssert.Contains" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "collection", collection );
                parameters.Add( "item", item );
                parameters.Add( "collectionName", collectionName );
                parameters.Add( "itemName", itemName );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="collectionName"></param>
        /// <param name="predicateText"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void Contains<T>(
            ICollection<T> collection,
            Func<T, bool> predicate,
            string collectionName,
            string predicateText )
        {
            FoundationContract.Requires<ArgumentNullException>( predicate != null );
            var enumerable = (IEnumerable<T>)collection;
            var indexedItem = enumerable.IndexOf( predicate );
            int index = indexedItem != null ? indexedItem.Index : -1;

            if (index < 0)
            {
                var message = new AssertMessage( "CollectionAssert.Contains" );
                var parameters = message.Parameters;
                parameters.Add( "collection", collection );
                parameters.Add( "predicate", predicate );
                parameters.Add( "collectionName", collectionName );
                parameters.Add( "predicateText", predicateText );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="keyName"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void Contains(
            IDictionary dictionary,
            object key,
            string dictionaryName,
            string keyName )
        {
            FoundationContract.Requires<ArgumentNullException>( dictionary != null );
            bool containsKey = dictionary.Contains( key );

            if (!containsKey)
            {
                AssertMessage message = new AssertMessage( "CollectionAssert.ContainsKey" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "dictionary", dictionary );
                parameters.Add( "dictionaryName", dictionaryName );
                parameters.Add( "key", key );
                parameters.Add( "keyName", keyName );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="keyName"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void ContainsKey<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            string dictionaryName,
            string keyName )
        {
            FoundationContract.Requires<ArgumentNullException>( dictionary != null );
            bool containsKey = dictionary.ContainsKey( key );

            if (!containsKey)
            {
                AssertMessage message = new AssertMessage( "CollectionAssert.ContainsKey" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "dictionary", dictionary );
                parameters.Add( "dictionaryName", dictionaryName );
                parameters.Add( "key", key );
                parameters.Add( "keyName", keyName );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="collectionName"></param>
        /// <param name="itemName"></param>
        public static void NotContains<T>(
            ICollection<T> collection,
            T item,
            string collectionName,
            string itemName )
        {
            FoundationContract.Requires<ArgumentNullException>( collection != null );
            bool contains = collection.Contains( item );

            if (contains)
            {
                var message = new AssertMessage( "CollectionAssert.Contains" );
                var parameters = message.Parameters;
                parameters.Add( "collection", collection );
                parameters.Add( "item", item );
                parameters.Add( "collectionName", collectionName );
                parameters.Add( "itemName", itemName );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="dictionaryName"></param>
        /// <param name="keyName"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void NotContainsKey<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            string dictionaryName,
            string keyName )
        {
            FoundationContract.Requires<ArgumentNullException>( dictionary != null );
            bool containsKey = dictionary.ContainsKey( key );

            if (containsKey)
            {
                var message = new AssertMessage( "CollectionAssert.NotContainsKey" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "dictionary", dictionary );
                parameters.Add( "dictionaryName", dictionaryName );
                parameters.Add( "key", key );
                parameters.Add( "keyName", keyName );
                Assert.Raise( message );
            }
        }
    }
}

#endif