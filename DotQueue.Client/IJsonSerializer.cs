namespace DotQueue.Client
{
    internal interface IJsonSerializer<T>
    {
        T Deserialize(string json);
        string Serialize(object item);
    }
}