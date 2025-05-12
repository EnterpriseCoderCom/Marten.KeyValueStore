using Marten.Schema;
using UUIDNext;

namespace EnterpriseCoder.Marten.KeyValueStore.Entities;

public class KeyValueEntry
{
    [Identity] public Guid Id { get; set; } = Uuid.NewDatabaseFriendly(Database.PostgreSql);

    public string TypeName { get; set; } = nameof(String);
    
    [UniqueIndex] [DuplicateField(NotNull = true)] 
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; } = string.Empty;
    
    
}