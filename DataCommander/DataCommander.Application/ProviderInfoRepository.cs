using System.Collections.Generic;
using Foundation.Configuration;

namespace DataCommander.Application;

public static class ProviderInfoRepository
{
    public static IEnumerable<ProviderInfo> GetProviderInfos()
    {
        var node = Settings.SelectNode("DataCommander/Providers", true)!;

        foreach (var childNode in node.ChildNodes)
        {
            childNode.Attributes.TryGetAttributeValue("Enabled", out bool enabled);
            if (enabled)
            {
                var identifier = childNode.Name!;

                if (!childNode.Attributes.TryGetAttributeValue("Name", out string? name))
                    name = identifier;

                var provider = new ProviderInfo(identifier, name!);
                yield return provider;
            }
        }
    }
}