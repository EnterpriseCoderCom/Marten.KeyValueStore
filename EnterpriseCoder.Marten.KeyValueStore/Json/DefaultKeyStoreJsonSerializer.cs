using Newtonsoft.Json;

namespace EnterpriseCoder.Marten.KeyValueStore.Json;

public class DefaultKeyStoreJsonSerializer : IJsonSerializer
{
    public static readonly DefaultKeyStoreJsonSerializer Instance = new();
    
    public string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None);
    }

    public T? Deserialize<T>(Type storedType, string json)
    {
        return (T?)JsonConvert.DeserializeObject(json, storedType);
    }
}