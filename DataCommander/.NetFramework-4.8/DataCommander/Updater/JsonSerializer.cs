using Foundation.Core;
using Newtonsoft.Json;

namespace DataCommander.Updater
{
    public sealed class JsonSerializer : ISerializer
    {
        public string Serialize(object objectGraph) =>
            JsonConvert.SerializeObject(
                objectGraph,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});

        public T Deserialize<T>(string serializedObjectGraph) =>
            JsonConvert.DeserializeObject<T>(serializedObjectGraph, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
    }
}