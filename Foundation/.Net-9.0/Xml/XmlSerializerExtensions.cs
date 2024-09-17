using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Xml;

public static class XmlSerializerExtensions
{
    public static string SerializeToXmlString(this XmlSerializer xmlSerializer, XmlWriterSettings xmlWriterSettings, object o)
    {
        ArgumentNullException.ThrowIfNull(xmlSerializer);

        StringBuilder stringBuilder = new StringBuilder();

        using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            xmlSerializer.Serialize(xmlWriter, o);

        return stringBuilder.ToString();
    }
}