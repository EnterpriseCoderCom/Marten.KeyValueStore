﻿namespace EnterpriseCoder.Marten.KeyValueStore.Json;

public interface IJsonSerializer
{
    string Serialize(object obj);
    T? Deserialize<T>(Type storedType, string json);
}