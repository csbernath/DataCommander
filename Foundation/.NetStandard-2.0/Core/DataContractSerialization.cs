using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Foundation.Core
{
    public static class DataContractSerialization
    {
        public static string Serialize<T>(T objectGraph)
        {
            var serializer = new DataContractSerializer(typeof(T));
            string xml;
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, objectGraph);
                xml = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return xml;
        }

        public static string Serialize<T>(T objectGraph, XmlWriterSettings xmlWriterSettings)
        {
            var stringBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xmlWriter, objectGraph);
            }

            var xml = stringBuilder.ToString();
            return xml;
        }

        public static T Deserialize<T>(string xml)
        {
            var serializer = new DataContractSerializer(typeof(T));
            T objectGraph;
            using (var xmlReader = XmlReader.Create(new StringReader(xml)))
                objectGraph = (T) serializer.ReadObject(xmlReader);
            return objectGraph;
        }
    }
}