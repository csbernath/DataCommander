using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Xml;

public static class XmlSerialization
{
    public static object Deserialize(string xml, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        StringReader stringReader = new StringReader(xml);
        XmlSerializer xmlSerializer = new XmlSerializer(type);
        object obj = xmlSerializer.Deserialize(stringReader);
        return obj;
    }

    public static object Deserialize(XmlReader xmlReader, Type type)
    {
        ArgumentNullException.ThrowIfNull(xmlReader);
        ArgumentNullException.ThrowIfNull(type);

        XmlSerializer xmlSerializer = new XmlSerializer(type);
        object obj = xmlSerializer.Deserialize(xmlReader);
        return obj;
    }

    public static T Deserialize<T>(string xml)
    {
        object obj = Deserialize(xml, typeof(T));
        return (T) obj;
    }

    public static T Deserialize<T>(XmlReader xmlReader)
    {
        object obj = Deserialize(xmlReader, typeof(T));
        return (T) obj;
    }

    public static string Serialize(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Type type = source.GetType();
        XmlSerializer xmlSerializer = new XmlSerializer(type);

        XmlWriterSettings settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true
        };

        return xmlSerializer.SerializeToXmlString(settings, source);
    }
}