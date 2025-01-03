using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Foundation.Assertions;

namespace Foundation.Xml;

public static class XmlSerializerExtensions
{
    public static string SerializeToXmlString(this XmlSerializer xmlSerializer, XmlWriterSettings xmlWriterSettings, object o)
    {
        Assert.IsNotNull(xmlSerializer);

        var stringBuilder = new StringBuilder();

        using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            xmlSerializer.Serialize(xmlWriter, o);

        return stringBuilder.ToString();
    }
}