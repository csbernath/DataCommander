using System.Diagnostics;

namespace Foundation.Configuration;

internal sealed class ConfigurationAttributeCollectionDebugger
{
    private readonly ConfigurationAttributeCollection _collection;

    public ConfigurationAttributeCollectionDebugger(ConfigurationAttributeCollection collection)
    {
        _collection = collection;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public ConfigurationAttribute[] Items
    {
        get
        {
            var array = new ConfigurationAttribute[_collection.Count];
            _collection.CopyTo(array, 0);
            return array;
        }
    }
}