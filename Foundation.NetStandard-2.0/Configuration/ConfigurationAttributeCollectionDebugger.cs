using System.Diagnostics;

namespace Foundation.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ConfigurationAttributeCollectionDebugger
    {
        private readonly ConfigurationAttributeCollection _collection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public ConfigurationAttributeCollectionDebugger( ConfigurationAttributeCollection collection )
        {
            _collection = collection;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public ConfigurationAttribute[] Items
        {
            get
            {
                var array = new ConfigurationAttribute[_collection.Count ];
                _collection.CopyTo( array, 0 );
                return array;
            }
        }
    }
}