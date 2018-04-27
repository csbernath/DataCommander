using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

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
            var serializer = new DataContractJsonSerializer(typeof(T));
            object objectGraph;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                objectGraph = serializer.ReadObject(memoryStream);
            return (T) objectGraph;
        }
    }
}