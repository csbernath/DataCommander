using System.Diagnostics;

namespace Foundation.Configuration;

internal sealed class ConfigurationAttributeCollectionDebugger(ConfigurationAttributeCollection collection)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public ConfigurationAttribute[] Items
    {
        get
        {
            ConfigurationAttribute[] array = new ConfigurationAttribute[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }
    }
}