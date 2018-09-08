using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Foundation
{
    public static class DataContractSerialization
    {
        public static string Serialize<T>(T objectGraph)
        {
            string xml;
            var serializer = new DataContractSerializer(typeof(T));
            using (var memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, objectGraph);
                xml = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return xml;
        }
    }
}