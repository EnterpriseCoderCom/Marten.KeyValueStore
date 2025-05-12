using EnterpriseCoder.Marten.KeyValueStore.Entities;
using Marten;

namespace EnterpriseCoder.Marten.KeyValueStore.UnitTests;

public class DatabaseHelper
{
    private readonly IDocumentSession _session;

    public DatabaseHelper(IDocumentSession session)
    {
        _session = session;
    }

    public async Task ClearDatabaseAsync()
    {
        _session.DeleteWhere<KeyValueEntry>(x => true);
        await _session.SaveChangesAsync();
    }

    public async Task<int> CountKeyValuesAsync()
    {
        return await _session.Query<KeyValueEntry>().CountAsync();
    }
}