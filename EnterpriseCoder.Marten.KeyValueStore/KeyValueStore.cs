using System.Globalization;
using EnterpriseCoder.Marten.KeyValueStore.Entities;
using EnterpriseCoder.Marten.KeyValueStore.Exceptions;
using EnterpriseCoder.Marten.KeyValueStore.Json;
using EnterpriseCoder.Marten.KeyValueStore.Procedures;
using Marten;

namespace EnterpriseCoder.Marten.KeyValueStore;

public class KeyValueStore : IKeyValueStore
{
    private readonly KeyValueEntryProcedures _keyValueEntryProcedures = new();

    private async Task<T> _GetAsyncWithConversion<T>(IDocumentSession documentSession, string key, T defaultValue,
        Func<string?, Type, T, T> valueParser)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry == null)
        {
            return defaultValue;
        }

        // Validate that this value was stored using the same type as it is being retrieved with.
        Type? storedType = _FindType(targetEntry.TypeName);
        if (storedType == null)
        {
            throw new KeyStoreValueException($"Unable to find type {targetEntry.TypeName}");
        }
        
        if( storedType.IsAssignableTo(typeof(T)) is false)
        {
            throw new KeyStoreValueException($"Value '{key}' was stored as type '{targetEntry.TypeName}' but is being retrieved as type '{typeof(T).FullName}'");
        }
        
        return valueParser(targetEntry.Value, storedType, defaultValue);
    }

    private async Task _SetAsyncWithConversion<T>(IDocumentSession documentSession, string key, T value,
        Func<T, string?> stringifier)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry == null)
        {
            targetEntry = new KeyValueEntry
            {
                Key = key,
                TypeName = typeof(T).FullName!
            };
        }

        string? valueToStore = stringifier(value);
        targetEntry.Value = valueToStore;

        _keyValueEntryProcedures.Store(documentSession, targetEntry);
    }

    public async Task<string?> GetStringAsync(IDocumentSession documentSession, string key, string? defaultValue)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry == null)
        {
            return defaultValue;
        }

        // Validate that this value was stored using the same type as it is being retrieved with.
        if (targetEntry.TypeName != nameof(String))
        {
            throw new KeyStoreValueException($"Value '{key}' was stored as type '{targetEntry.TypeName}' but is being retrieved as type '{nameof(String)}'");
        }

        return targetEntry.Value;
    }

    public async Task<bool> GetBoolAsync(IDocumentSession documentSession, string key, bool defaultValue)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, _, inDefault) =>
        {
            if (!bool.TryParse(inString, out var result))
            {
                return inDefault;
            }

            return result;
        });
    }

    public async Task<int> GetIntAsync(IDocumentSession documentSession, string key, int defaultValue)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, _, inDefault) =>
        {
            if (!int.TryParse(inString, out var result))
            {
                return inDefault;
            }

            return result;
        });
    }

    public async Task<long> GetLongAsync(IDocumentSession documentSession, string key, long defaultValue)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, _, inDefault) =>
        {
            if (!long.TryParse(inString, out var result))
            {
                return inDefault;
            }

            return result;
        });
    }

    public async Task<float> GetFloatAsync(IDocumentSession documentSession, string key, float defaultValue)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, _, inDefault) =>
        {
            if (!float.TryParse(inString, out var result))
            {
                return inDefault;
            }

            return result;
        });
    }

    public async Task<double> GetDoubleAsync(IDocumentSession documentSession, string key, double defaultValue)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, _, inDefault) =>
        {
            if (!double.TryParse(inString, out var result))
            {
                return inDefault;
            }

            return result;
        });
    }

    public async Task<T?> GetObjectAsync<T>(IDocumentSession documentSession, string key, T? defaultValue,
        IJsonSerializer? jsonSerializer = null)
    {
        return await _GetAsyncWithConversion(documentSession, key, defaultValue, (inString, storedType, inDefault) =>
        {
            if (inString == null)
            {
                return inDefault;
            }
            
            var jsonSerializerToUse = jsonSerializer ?? DefaultKeyStoreJsonSerializer.Instance;
            T? result = jsonSerializerToUse.Deserialize<T>(storedType, inString);
            if (result == null)
            {
                return inDefault;
            }
            
            return result;
        });
        
    }

    public async Task SetStringAsync(IDocumentSession documentSession, string key, string? value)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry == null)
        {
            targetEntry = new KeyValueEntry
            {
                Key = key,
                TypeName = nameof(String)
            };
        }

        targetEntry.Value = value;
        _keyValueEntryProcedures.Store(documentSession, targetEntry);
    }

    public async Task SetBoolAsync(IDocumentSession documentSession, string key, bool value)
    {
        await _SetAsyncWithConversion(documentSession, key, value, inValue => inValue.ToString());
    }

    public async Task SetIntAsync(IDocumentSession documentSession, string key, int value)
    {
        await _SetAsyncWithConversion(documentSession, key, value, inValue => inValue.ToString());
    }

    public async Task SetLongAsync(IDocumentSession documentSession, string key, long value)
    {
        await _SetAsyncWithConversion(documentSession, key, value, inValue => inValue.ToString());
    }

    public async Task SetFloatAsync(IDocumentSession documentSession, string key, float value)
    {
        await _SetAsyncWithConversion(documentSession, key, value,
            inValue => inValue.ToString(CultureInfo.InvariantCulture));
    }

    public async Task SetDoubleAsync(IDocumentSession documentSession, string key, double value)
    {
        await _SetAsyncWithConversion(documentSession, key, value,
            inValue => inValue.ToString(CultureInfo.InvariantCulture));
    }

    public async Task SetObjectAsync<T>(IDocumentSession documentSession, string key, T value, IJsonSerializer? jsonSerializer = null)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry == null)
        {
            targetEntry = new KeyValueEntry
            {
                Key = key
            };
        }

        IJsonSerializer serializerToUse = jsonSerializer ?? DefaultKeyStoreJsonSerializer.Instance;

        targetEntry.Value = serializerToUse.Serialize(value);
        targetEntry.TypeName = value.GetType().FullName!;
        _keyValueEntryProcedures.Store(documentSession, targetEntry);
        
    }

    public async Task<bool> HasKeyAsync(IDocumentSession documentSession, string key)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        return targetEntry != null;
    }

    public async Task DeleteKeyAsync(IDocumentSession documentSession, string key)
    {
        var targetEntry = await _keyValueEntryProcedures.GetValueAsync(documentSession, key);
        if (targetEntry != null)
        {
            _keyValueEntryProcedures.Delete(documentSession, targetEntry);
        }
    }

    private Type? _FindType(string typeName)
    {
        foreach (var nextAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? returnType = nextAssembly.GetType(typeName);
            if (returnType != null)
            {
                return returnType;
            }
        }

        return null;
    }
}