using System.Collections.Generic;
using Foundation.Configuration;

namespace DataCommander.Application;

public static class ProviderInfoRepository
{
    public static IEnumerable<ProviderInfo> GetProviderInfos()
    {
        ConfigurationNode node = Settings.SelectNode("DataCommander/Providers", true);

        foreach (ConfigurationNode? childNode in node.ChildNodes)
        {
            childNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);
            if (enabled)
            {
                string identifier = childNode.Name;

                if (!childNode.Attributes.TryGetAttributeValue("Name", out string name))
                    name = identifier;

                ProviderInfo provider = new ProviderInfo(identifier, name);
                yield return provider;
            }
        }
    }
}