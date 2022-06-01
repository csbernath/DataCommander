using System;
using System.Xml;
using Foundation.Assertions;
using Foundation.Core;

namespace Foundation.Xml;

public static class XmlWriterExtensions
{
    public static IDisposable WriteElement(this XmlWriter xmlWriter, string localName)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);
        xmlWriter.WriteStartElement(localName);
        return new Disposer(xmlWriter.WriteEndElement);
    }

    public static IDisposable WriteElement(this XmlWriter xmlWriter, string prefix, string localName, string ns)
    {
        ArgumentNullException.ThrowIfNull(xmlWriter);
        xmlWriter.WriteStartElement(prefix, localName, ns);
        return new Disposer(xmlWriter.WriteEndElement);
    }
}