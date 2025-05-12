using EnterpriseCoder.Marten.KeyValueStore.CompiledQueries;
using EnterpriseCoder.Marten.KeyValueStore.Entities;
using Marten;

namespace EnterpriseCoder.Marten.KeyValueStore.Procedures;

public class KeyValueEntryProcedures
{
    public async Task<KeyValueEntry?> GetValueAsync(IDocumentSession documentSession, string key)
    {
        return await documentSession.QueryAsync(new QuerySelectKeyValueEntry() { Key = key });
    }

    public void Store(IDocumentSession documentSession, KeyValueEntry entry)
    {
        documentSession.Store(entry);
    }

    public void Delete(IDocumentSession documentSession, KeyValueEntry entry)
    {
        documentSession.HardDelete(entry);
    }
}