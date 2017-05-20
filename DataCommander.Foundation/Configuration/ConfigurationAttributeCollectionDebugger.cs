namespace DataCommander.Foundation.Configuration
{
    using System.Diagnostics;

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
            this._collection = collection;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public ConfigurationAttribute[] Items
        {
            get
            {
                var array = new ConfigurationAttribute[ this._collection.Count ];
                this._collection.CopyTo( array, 0 );
                return array;
            }
        }
    }
}