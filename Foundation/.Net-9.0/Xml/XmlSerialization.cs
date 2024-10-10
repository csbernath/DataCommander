using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Foundation.Xml;

public static class XmlSerialization
{
    public static object? Deserialize(string xml, Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var stringReader = new StringReader(xml);
        var xmlSerializer = new XmlSerializer(type);
        var obj = xmlSerializer.Deserialize(stringReader);
        return obj;
    }

    public static object? Deserialize(XmlReader xmlReader, Type type)
    {
        ArgumentNullException.ThrowIfNull(xmlReader);
        ArgumentNullException.ThrowIfNull(type);

        var xmlSerializer = new XmlSerializer(type);
        var obj = xmlSerializer.Deserialize(xmlReader);
        return obj;
    }

    public static T? Deserialize<T>(string xml)
    {
        var obj = Deserialize(xml, typeof(T));
        return (T?)obj;
    }

    public static T? Deserialize<T>(XmlReader xmlReader)
    {
        var obj = Deserialize(xmlReader, typeof(T));
        return (T?)obj;
    }

    public static string Serialize(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var type = source.GetType();
        var xmlSerializer = new XmlSerializer(type);

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true
        };

        return xmlSerializer.SerializeToXmlString(settings, source);
    }
}