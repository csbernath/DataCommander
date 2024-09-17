using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Foundation.Core;

public static class DataContractJsonSerialization
{
    public static string Serialize<T>(T objectGraph)
    {
        string json;
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        using (MemoryStream memoryStream = new MemoryStream())
        {
            serializer.WriteObject(memoryStream, objectGraph);
            json = Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        return json;
    }

    public static T Deserialize<T>(string json)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        object objectGraph;
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            objectGraph = serializer.ReadObject(memoryStream);
        return (T) objectGraph;
    }
}