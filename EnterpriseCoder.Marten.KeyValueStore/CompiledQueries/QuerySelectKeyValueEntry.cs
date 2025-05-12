using System.Linq.Expressions;
using EnterpriseCoder.Marten.KeyValueStore.Entities;
using Marten.Linq;

namespace EnterpriseCoder.Marten.KeyValueStore.CompiledQueries;

public class QuerySelectKeyValueEntry : ICompiledQuery<KeyValueEntry, KeyValueEntry?>
{
    public string Key { get; set; } = string.Empty;
    
    public Expression<Func<IMartenQueryable<KeyValueEntry>, KeyValueEntry?>> QueryIs()
    {
        return q => q.SingleOrDefault(x => x.Key == Key);
    }
}