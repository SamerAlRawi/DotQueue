using Newtonsoft.Json;

namespace DotQueue.Client
{
    internal class JsonSerializer<T> : IJsonSerializer<T>
    {
        public T Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}