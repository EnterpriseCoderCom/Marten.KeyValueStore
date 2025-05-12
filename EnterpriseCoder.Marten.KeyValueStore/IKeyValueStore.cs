using EnterpriseCoder.Marten.KeyValueStore.Json;
using Marten;

namespace EnterpriseCoder.Marten.KeyValueStore;

public interface IKeyValueStore
{
    Task<bool> HasKeyAsync(IDocumentSession documentSession, string key);
    Task DeleteKeyAsync(IDocumentSession documentSession, string key);
    
    Task<string?> GetStringAsync(IDocumentSession documentSession, string key, string? defaultValue);
    Task<bool> GetBoolAsync(IDocumentSession documentSession, string key, bool defaultValue);
    Task<int> GetIntAsync(IDocumentSession documentSession, string key, int defaultValue);
    Task<long> GetLongAsync(IDocumentSession documentSession, string key, long defaultValue);
    Task<float> GetFloatAsync(IDocumentSession documentSession, string key, float defaultValue);
    Task<double> GetDoubleAsync(IDocumentSession documentSession, string key, double defaultValue);

    Task<T?> GetObjectAsync<T>(IDocumentSession documentSession, string key, T? defaultValue,
        IJsonSerializer? jsonSerializer = null);
    
    Task SetStringAsync(IDocumentSession documentSession, string key, string? value);
    Task SetBoolAsync(IDocumentSession documentSession, string key, bool value);
    Task SetIntAsync(IDocumentSession documentSession, string key, int value);
    Task SetLongAsync(IDocumentSession documentSession, string key, long value);
    Task SetFloatAsync(IDocumentSession documentSession, string key, float value);
    Task SetDoubleAsync(IDocumentSession documentSession, string key, double value);

    Task SetObjectAsync<T>(IDocumentSession documentSession, string key, T value,
        IJsonSerializer? jsonSerializer = null);
}