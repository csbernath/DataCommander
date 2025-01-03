using System;
using System.Xml;

namespace Foundation.Configuration;

public sealed class XmlAttributeReader
{
    private readonly XmlAttributeCollection _attributes;

    public XmlAttributeReader(XmlAttributeCollection attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        _attributes = attributes;
    }

    public static bool TryGetValue(XmlAttributeCollection attributes, string name, out string value)
    {
        ArgumentNullException.ThrowIfNull(attributes, nameof(attributes));
        var attribute = attributes[name];
        var contains = attribute != null;
        value = contains ? attribute.Value : null;
        return contains;
    }

    public bool TryGetValue(string name, out string value) => TryGetValue(_attributes, name, out value);
}