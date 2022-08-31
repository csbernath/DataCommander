using System;
using System.Xml;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetAttribute
{
    public XmlSpreadsheetAttribute(string localName, string value)
    {
        LocalName = localName;
        Value = value;
    }

    public string LocalName { get; }

    public string Value { get; }

    public void Write(XmlWriter xmlWriter)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);

        xmlWriter.WriteAttributeString(LocalName, Value);
    }
}