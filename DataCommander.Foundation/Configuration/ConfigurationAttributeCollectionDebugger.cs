namespace DataCommander.Foundation.Configuration
{
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class ConfigurationAttributeCollectionDebugger
    {
        private readonly ConfigurationAttributeCollection collection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public ConfigurationAttributeCollectionDebugger( ConfigurationAttributeCollection collection )
        {
            this.collection = collection;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public ConfigurationAttribute[] Items
        {
            get
            {
                var array = new ConfigurationAttribute[ this.collection.Count ];
                this.collection.CopyTo( array, 0 );
                return array;
            }
        }
    }
}