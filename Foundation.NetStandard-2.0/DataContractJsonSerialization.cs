using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Foundation
{
    public static class DataContractJsonSerialization
    {
        public static string Serialize<T>(T objectGraph)
        {
            string json;
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, objectGraph);
                json = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return json;
        }

        public static T Deserialize<T>(string json)
        {
            object objectGraph;
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var textReader = new StringReader(json))
            using (var xmlReader = new XmlTextReader(textReader))
                objectGraph = serializer.ReadObject(xmlReader, true);

            return (T) objectGraph;
        }
    }
}