namespace Foundation.Core
{
    public interface ISerializer
    {
        string Serialize(object objectGraph);
        T Deserialize<T>(string serializedObjectGraph);
    }
}