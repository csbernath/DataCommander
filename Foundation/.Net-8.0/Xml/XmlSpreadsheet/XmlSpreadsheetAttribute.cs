using System;
using System.Xml;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetAttribute(string localName, string value)
{
    public string LocalName { get; } = localName;

    public string Value { get; } = value;

    public void Write(XmlWriter xmlWriter)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);

        xmlWriter.WriteAttributeString(LocalName, Value);
    }
}