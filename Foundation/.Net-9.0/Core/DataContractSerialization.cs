using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Foundation.Core;

public static class DataContractSerialization
{
    public static string Serialize<T>(T objectGraph)
    {
        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
        string xml;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            serializer.WriteObject(memoryStream, objectGraph);
            xml = Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        return xml;
    }

    public static string Serialize<T>(T objectGraph, XmlWriterSettings xmlWriterSettings)
    {
        StringBuilder stringBuilder = new StringBuilder();
        using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(xmlWriter, objectGraph);
        }

        string xml = stringBuilder.ToString();
        return xml;
    }

    public static T Deserialize<T>(string xml)
    {
        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
        T objectGraph;
        using (XmlReader xmlReader = XmlReader.Create(new StringReader(xml)))
            objectGraph = (T) serializer.ReadObject(xmlReader);
        return objectGraph;
    }
}