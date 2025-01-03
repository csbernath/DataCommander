using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Assertions;

namespace Foundation.Xml;

public static class XmlSerialization
{
    public static object Deserialize(string xml, Type type)
    {
        Assert.IsNotNull(type);

        var stringReader = new StringReader(xml);
        var xmlSerializer = new XmlSerializer(type);
        var obj = xmlSerializer.Deserialize(stringReader);
        return obj;
    }

    public static object Deserialize(XmlReader xmlReader, Type type)
    {
        Assert.IsNotNull(xmlReader);
        Assert.IsNotNull(type);

        var xmlSerializer = new XmlSerializer(type);
        var obj = xmlSerializer.Deserialize(xmlReader);
        return obj;
    }

    public static T Deserialize<T>(string xml)
    {
        var obj = Deserialize(xml, typeof(T));
        return (T) obj;
    }

    public static T Deserialize<T>(XmlReader xmlReader)
    {
        var obj = Deserialize(xmlReader, typeof(T));
        return (T) obj;
    }

    public static string Serialize(object source)
    {
        Assert.IsNotNull(source);
        var type = source.GetType();
        var xmlSerializer = new XmlSerializer(type);

        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true
        };

        return xmlSerializer.SerializeToXmlString(settings, source);
    }
}