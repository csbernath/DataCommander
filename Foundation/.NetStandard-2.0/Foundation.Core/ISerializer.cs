namespace Foundation
{
    public interface ISerializer
    {
        string Serialize(object objectGraph);
        T Deserialize<T>(string serializedObjectGraph);
    }
}