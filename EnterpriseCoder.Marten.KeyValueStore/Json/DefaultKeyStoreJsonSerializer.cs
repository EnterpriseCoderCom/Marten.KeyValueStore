using Newtonsoft.Json;

namespace EnterpriseCoder.Marten.KeyValueStore.Json;

public class DefaultKeyStoreJsonSerializer : IJsonSerializer
{
    public static readonly DefaultKeyStoreJsonSerializer Instance = new();
    
    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}