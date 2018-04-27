using Foundation;
using Newtonsoft.Json;

namespace DataCommander.Updater
{
    public sealed class JsonSerializer : ISerializer
    {
        public string Serialize(object objectGraph) => JsonConvert.SerializeObject(objectGraph);
        public T Deserialize<T>(string serializedObjectGraph) => JsonConvert.DeserializeObject<T>(serializedObjectGraph);
    }
}